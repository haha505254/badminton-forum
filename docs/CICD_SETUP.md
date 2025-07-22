# CI/CD 設定文檔

## 概述
本專案使用 GitHub Actions 自動化部署到 AWS EC2，使用 ECR 儲存 Docker images。採用 Nginx 反向代理架構，提供高安全性和零中斷部署。

## 架構
```
GitHub → GitHub Actions → AWS ECR → EC2 (Nginx + Docker Compose)
```

## 已完成的設定

### 1. AWS 資源
- **ECR Repositories**:
  - `badminton-forum-api`: API Docker image
  - `badminton-forum-frontend`: Frontend Docker image
  
- **EC2 實例**:
  - Elastic IP: 34.199.113.111
  - Type: t3.micro
  - OS: Ubuntu 24.04
  - 架構: Nginx 反向代理 + Docker Compose

- **Systems Manager 參數**:
  - `/badminton-forum/db-password`: 資料庫密碼
  - `/badminton-forum/jwt-secret`: JWT 密鑰

### 2. GitHub Secrets 需要設定
請在 GitHub Repository Settings → Secrets and variables → Actions 中新增：

- `AWS_ACCESS_KEY_ID`: 你的 AWS Access Key ID
- `AWS_SECRET_ACCESS_KEY`: 你的 AWS Secret Access Key
- `EC2_HOST`: 34.199.113.111
- `EC2_USERNAME`: ubuntu
- `EC2_SSH_KEY`: 你的 EC2 私鑰內容（完整的 .pem 檔案內容）

### 3. 部署檔案
- `.github/workflows/deploy-aws.yml`: GitHub Actions workflow
- `deploy/deploy.sh`: EC2 部署腳本（支援零中斷部署）
- `deploy/docker-compose.prod.yml`: 生產環境 Docker Compose 設定

## 部署流程

1. **Push 到 main 分支** → 觸發 GitHub Actions
2. **執行測試** → 使用 PostgreSQL 服務容器進行 API 測試
3. **建置 Docker images**：
   - API: 使用精簡的 aspnet:8.0 runtime image
   - Frontend: 注入相對路徑 `/api` 作為 API URL
4. **推送到 ECR** → 儲存 images 並標記為 latest
5. **SSH 到 EC2** → 執行部署腳本
6. **零中斷更新** → 使用 `docker-compose up -d --remove-orphans`
7. **健康檢查** → 確認服務正常運行

## 安全架構

### Nginx 反向代理
所有外部請求都經過 Nginx（Port 80），再轉發到內部服務：
- `/` → 前端靜態檔案
- `/api` → 後端 API (內部 port 5246)

### API 安全設定
- API 不對外暴露端口
- CORS 使用環境變數 `FRONTEND_URL` 動態設定
- JWT 密鑰存放在 AWS Systems Manager

## 環境變數配置

### 生產環境 (deploy/docker-compose.prod.yml)
```yaml
api:
  environment:
    ASPNETCORE_ENVIRONMENT: Production
    ASPNETCORE_URLS: http://+:5246
    ConnectionStrings__DefaultConnection: "Host=db;..."
    JwtSettings__Secret: ${JWT_SECRET}
    FRONTEND_URL: http://34.199.113.111
```

### 前端配置
- API URL 在 build time 注入：`VITE_API_URL=/api`
- 不使用 `.env.production` 避免配置混亂

## 手動操作

### 部署
```bash
ssh ubuntu@34.199.113.111
cd ~/badminton-forum
./deploy.sh
```

### 資料庫遷移
由於生產環境不包含 EF 工具，需要另外處理：
```bash
# 選項 1: 使用開發環境 container
docker-compose exec api dotnet ef database update

# 選項 2: 從 CI/CD pipeline 執行
# 在 deploy 前加入遷移步驟
```

### 查看日誌
```bash
docker-compose -f docker-compose.prod.yml logs -f api
docker-compose -f docker-compose.prod.yml logs -f web
```

## 監控
- 前端: http://34.199.113.111
- API 健康檢查: 透過 Nginx 訪問 http://34.199.113.111/api/health

## 注意事項

1. **AWS IAM 權限**：
   - ECR: push/pull images
   - SSM: 讀取參數

2. **EC2 安全群組**：
   - Port 22 (SSH) - 限制來源 IP
   - Port 80 (HTTP) - 對外開放
   - ~~Port 5246~~ (API 已不需要對外開放)

3. **首次部署**：
   - 確保 Systems Manager 參數已設定
   - 手動執行資料庫遷移
   - 檢查 FRONTEND_URL 環境變數

4. **更新 IP 或域名時**：
   - 更新 `FRONTEND_URL` 環境變數
   - 更新 GitHub Secrets 中的 `EC2_HOST`
   - 重新部署以套用變更

## 故障排除

### API 無法訪問
1. 檢查 Nginx 配置：`docker-compose exec web cat /etc/nginx/conf.d/default.conf`
2. 確認 API container 運行中：`docker-compose ps`
3. 查看 API 日誌：`docker-compose logs api`

### CORS 錯誤
1. 確認 `FRONTEND_URL` 環境變數設定正確
2. 重啟 API 服務：`docker-compose restart api`

### 部署失敗
1. 檢查 GitHub Actions 日誌
2. 確認 AWS 憑證有效
3. 檢查 ECR 容量限制