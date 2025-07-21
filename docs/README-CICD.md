# CI/CD 流程說明

## 概述
本專案使用 GitHub Actions 實作完整的 CI/CD 流程，包含自動化測試、建置、安全掃描和部署。

## 工作流程檔案

### 1. CI/CD Pipeline (`.github/workflows/ci.yml`)
主要的 CI/CD 流程，包含：

- **API 測試**
  - 設定 .NET 8.0 環境
  - 執行單元測試
  - 使用 PostgreSQL 服務容器進行整合測試

- **前端測試**
  - 設定 Node.js 18 環境
  - 執行單元測試（如果有）
  - 建置前端應用

- **E2E 測試**
  - 使用 Playwright 執行端對端測試
  - 啟動完整的應用堆疊
  - 測試失敗時上傳測試報告

- **Docker 映像建置與推送**
  - 建置 API 和前端的 Docker 映像
  - 推送至 GitHub Container Registry (ghcr.io)
  - 使用分支和 commit SHA 作為標籤

### 2. 部署流程 (`.github/workflows/cd.yml`)
- **Staging 部署**
  - 自動部署到測試環境
  - 執行健康檢查

- **Production 部署**
  - 需要手動批准
  - 部署到生產環境
  - Slack 通知

### 3. 安全掃描 (`.github/workflows/security.yml`)
- **.NET 套件漏洞掃描**
- **NPM 套件安全審計**
- **Docker 映像安全掃描（Trivy）**
- **程式碼品質檢查**

## 設定步驟

### 1. GitHub Secrets 設定
在 GitHub 儲存庫設定以下 Secrets：

```
# 部署相關
DEPLOY_HOST          # 生產環境主機
DEPLOY_USER          # SSH 使用者名稱
DEPLOY_KEY           # SSH 私鑰
STAGING_HOST         # 測試環境主機
STAGING_USER         # 測試環境使用者
STAGING_KEY          # 測試環境 SSH 私鑰
STAGING_URL          # 測試環境 URL
PROD_HOST            # 生產環境主機
PROD_USER            # 生產環境使用者
PROD_KEY             # 生產環境 SSH 私鑰
PROD_URL             # 生產環境 URL

# 通知相關
SLACK_WEBHOOK        # Slack Webhook URL
```

### 2. 環境變數設定
在部署伺服器上建立 `.env` 檔案：

```bash
# 資料庫設定
POSTGRES_USER=badmintonuser
POSTGRES_PASSWORD=your_secure_password
POSTGRES_DB=badmintonforumdb

# API 設定
CONNECTION_STRING="Host=postgres;Database=badmintonforumdb;Username=badmintonuser;Password=your_secure_password"
JWT_SECRET_KEY=your_jwt_secret_key
JWT_ISSUER=https://your-domain.com
JWT_AUDIENCE=https://your-domain.com

# 前端設定
VITE_API_URL=https://api.your-domain.com

# GitHub Container Registry
GITHUB_REPOSITORY=your-username/badminton-forum
```

### 3. 部署伺服器準備

1. 安裝 Docker 和 Docker Compose：
```bash
# 安裝 Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# 安裝 Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
```

2. 建立專案目錄：
```bash
sudo mkdir -p /opt/badminton-forum
sudo chown $USER:$USER /opt/badminton-forum
```

3. 複製 docker-compose 檔案：
```bash
cp docker-compose.prod.yml /opt/badminton-forum/
cp docker-compose.staging.yml /opt/badminton-forum/
```

4. 設定 GitHub Container Registry 認證：
```bash
echo $GITHUB_TOKEN | docker login ghcr.io -u $GITHUB_USER --password-stdin
```

## 觸發條件

- **CI/CD Pipeline**：推送到 `main` 或 `develop` 分支時觸發
- **部署**：CI/CD Pipeline 成功完成後觸發
- **安全掃描**：每週一凌晨 2 點定期執行

## 本地測試

### 執行測試
```bash
# API 測試
cd BadmintonForum.API
dotnet test

# 前端 E2E 測試
cd badminton-forum-vue
npm run test:e2e
```

### 建置 Docker 映像
```bash
# API
docker build -t badminton-api ./BadmintonForum.API

# 前端
docker build -t badminton-frontend ./badminton-forum-vue
```

### 本地執行
```bash
docker-compose up -d
```

## 監控與維護

1. **查看部署狀態**
   - GitHub Actions 頁面查看工作流程執行狀態
   - Slack 通知查看部署結果

2. **日誌查看**
   ```bash
   # 查看容器日誌
   docker-compose logs -f api
   docker-compose logs -f frontend
   ```

3. **健康檢查**
   ```bash
   curl http://your-domain.com/health
   ```

## 故障排除

1. **部署失敗**
   - 檢查 GitHub Secrets 設定
   - 確認 SSH 連線正常
   - 查看 GitHub Actions 日誌

2. **測試失敗**
   - 下載 Playwright 測試報告
   - 檢查服務是否正常啟動
   - 查看測試日誌

3. **Docker 映像建置失敗**
   - 確認 Dockerfile 語法正確
   - 檢查基礎映像是否可用
   - 查看建置日誌