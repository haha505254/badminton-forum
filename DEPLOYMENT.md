# 部署指南

## 🚀 快速部署

### 開發環境（本地）

```bash
# 1. 複製環境設定
cp .env.defaults .env

# 2. 啟動服務
docker compose up

# 3. 訪問服務
# 前端: http://localhost:5173
# 管理後台: http://localhost:5174
# API: http://localhost:5246/swagger
```

### 生產環境（EC2/伺服器）

#### 方法一：使用一鍵部署腳本（推薦）

```bash
# 1. Clone 專案
git clone https://github.com/你的帳號/badminton-forum.git
cd badminton-forum

# 2. 複製並修改環境設定
cp .env.defaults .env
nano .env  # 修改必要的環境變數（見下方說明）

# 3. 執行部署腳本
./deploy.sh
```

#### 方法二：手動部署

```bash
# 1. 複製環境設定
cp .env.defaults .env

# 2. 修改 .env 設定（使用 nano 或 vim）
# 必須修改的變數：
# SERVER_IP=15.168.229.18  # 你的伺服器 IP
# VITE_API_URL=http://15.168.229.18:5246/api
# ALLOWED_ORIGINS=http://15.168.229.18:5173,http://15.168.229.18:5174
# EMAIL_BASE_URL=http://15.168.229.18:5173

# 3. 使用生產配置啟動（注意：必須加 --build）
docker compose -f docker-compose.prod.yml up -d --build

# 4. 檢查服務狀態
docker compose -f docker-compose.prod.yml ps
```

## 📋 環境變數說明

### 必須設定的變數（生產環境）

| 變數名 | 說明 | 範例 |
|--------|------|------|
| `SERVER_IP` | 伺服器 IP 或域名 | `15.168.229.18` |
| `VITE_API_URL` | 前端連接 API 的 URL（編譯時使用） | `http://15.168.229.18:5246/api` |
| `ALLOWED_ORIGINS` | 允許的 CORS 來源 | `http://15.168.229.18:5173,http://15.168.229.18:5174` |
| `EMAIL_BASE_URL` | 郵件中連結的基礎 URL | `http://15.168.229.18:5173` |

### 強烈建議修改的變數（安全性）

| 變數名 | 說明 | 預設值（必須更改） |
|--------|------|------------------|
| `JWT_SECRET` | JWT 加密金鑰 | `ThisIsAVerySecretKey...` |
| `MARIADB_PASSWORD` | 資料庫密碼 | `BadmintonPass123` |
| `MARIADB_ROOT_PASSWORD` | 資料庫 root 密碼 | `rootpass123` |
| `DEFAULT_ADMIN_EMAIL` | 管理員信箱 | `admin@badminton-forum.com` |
| `DEFAULT_ADMIN_PASSWORD` | 管理員密碼 | `Admin123456!` |

## 🔧 環境差異

### 開發環境
- 使用 `docker-compose.yml`
- 包含熱重載（Hot Reload）
- 程式碼掛載為 volumes
- 前端使用 Vite 開發伺服器（端口 5173, 5174）
- 資料庫端口暴露（3306）
- 可選用 Adminer 資料庫管理介面

### 生產環境
- 使用獨立的 `docker-compose.prod.yml`
- 使用最佳化的生產建置
- 無程式碼掛載（使用 Docker 映像內的編譯檔案）
- 前端使用 Nginx 靜態伺服器（容器內 80 端口）
- 資料庫端口不對外暴露
- 所有服務設定 `restart: always`

## ⚠️ 重要注意事項

### 1. 端口映射說明
生產環境的端口映射：
- `5173:80` - 主機 5173 映射到容器內 Nginx 的 80 端口
- `5174:80` - 主機 5174 映射到容器內 Nginx 的 80 端口
- `5246:5246` - API 端口直接映射

### 2. 前端 API URL 設定
**關鍵**：`VITE_API_URL` 必須在 Docker **建置階段**設定，而非運行時：
- ✅ 正確：在 `.env` 設定後使用 `--build` 重新建置
- ❌ 錯誤：只重啟容器而不重新建置

### 3. Docker Compose 版本
- 新版 Docker 使用 `docker compose`（空格）
- 舊版使用 `docker-compose`（連字號）
- deploy.sh 腳本已處理此差異

## 📝 常用命令

### 基本操作
```bash
# 查看服務狀態
docker compose -f docker-compose.prod.yml ps

# 查看日誌
docker compose -f docker-compose.prod.yml logs -f [service-name]

# 重啟特定服務
docker compose -f docker-compose.prod.yml restart [service-name]

# 停止所有服務
docker compose -f docker-compose.prod.yml down

# 重新建置並啟動（更新代碼後）
docker compose -f docker-compose.prod.yml up -d --build
```

### 進入容器
```bash
docker compose -f docker-compose.prod.yml exec api bash
docker compose -f docker-compose.prod.yml exec web sh
docker compose -f docker-compose.prod.yml exec admin sh
docker compose -f docker-compose.prod.yml exec db mariadb -u badmintonuser -p
```

### 更新部署
```bash
# 拉取最新代碼
git pull

# 使用部署腳本
./deploy.sh

# 或手動重新建置
docker compose -f docker-compose.prod.yml down
docker compose -f docker-compose.prod.yml up -d --build
```

## 🔍 故障排除

### 前端無法顯示或請求 localhost
**問題**：前端請求 `http://localhost:5246/api` 而非伺服器 IP

**解決方案**：
1. 確認 `.env` 中 `VITE_API_URL` 設定正確
2. 使用 `--build` 參數重新建置：
   ```bash
   docker compose -f docker-compose.prod.yml up -d --build
   ```
3. 清除瀏覽器快取

### 容器顯示 unhealthy 狀態
**診斷步驟**：
```bash
# 檢查容器內部端口
docker compose -f docker-compose.prod.yml exec web netstat -tlnp

# 檢查 Nginx 配置
docker compose -f docker-compose.prod.yml exec web cat /etc/nginx/conf.d/default.conf

# 測試容器內部服務
docker inspect badminton-forum-web -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' | xargs -I {} curl -I http://{}:80
```

### CORS 錯誤
**問題**：瀏覽器顯示 CORS policy 錯誤

**解決方案**：
1. 確認 `ALLOWED_ORIGINS` 包含正確的前端 URL
2. 確保 URL 格式正確（包含 http:// 和端口）
3. 重啟 API 服務：
   ```bash
   docker compose -f docker-compose.prod.yml restart api
   ```

### 資料庫連線失敗
```bash
# 檢查資料庫狀態
docker compose -f docker-compose.prod.yml exec db mariadb -u badmintonuser -pBadmintonPass123 -e "SELECT 1"

# 查看資料庫日誌
docker compose -f docker-compose.prod.yml logs db
```

### Docker Compose 命令找不到
**錯誤**：`docker-compose: command not found`

**解決方案**：使用新版格式
```bash
# 舊版（連字號）
docker-compose up

# 新版（空格）
docker compose up
```

## 👤 預設管理員帳號

系統會在首次啟動時自動創建管理員帳號。

### 預設值
- **Email**: admin@badminton-forum.com
- **Password**: Admin123456!

### 生產環境設定
**重要**：生產環境部署前必須修改 `.env` 中的管理員帳號密碼：
```bash
DEFAULT_ADMIN_EMAIL=your-admin@domain.com
DEFAULT_ADMIN_PASSWORD=YourSecurePassword123!
```

### 管理員功能
管理員可以：
- 存取管理後台 (http://你的IP:5174)
- 管理用戶（啟用/停用、授予管理員權限）
- 管理貼文（置頂、鎖定、刪除）
- 管理分類（新增、編輯、刪除）
- 管理回覆（軟刪除、批次刪除）

## 💾 備份與還原

### 備份資料庫
```bash
# 備份到檔案
docker compose -f docker-compose.prod.yml exec db mysqldump -u badmintonuser -pBadmintonPass123 badmintonforumdb > backup_$(date +%Y%m%d_%H%M%S).sql

# 壓縮備份
tar -czf backup_$(date +%Y%m%d).tar.gz backup_*.sql
```

### 還原資料庫
```bash
# 還原備份
docker compose -f docker-compose.prod.yml exec -T db mysql -u badmintonuser -pBadmintonPass123 badmintonforumdb < backup.sql
```

### 備份 Docker Volumes
```bash
# 備份整個資料卷
docker run --rm -v badminton-forum_mariadb_data:/data -v $(pwd):/backup alpine tar czf /backup/mariadb_data_$(date +%Y%m%d).tar.gz -C /data .
```

## 🔐 安全性建議

### 生產環境檢查清單
- [ ] 更改所有預設密碼（JWT_SECRET、MARIADB_PASSWORD、DEFAULT_ADMIN_PASSWORD）
- [ ] 設定防火牆規則，只開放必要端口（5173, 5174, 5246）
- [ ] 使用 HTTPS/SSL（見下方 Nginx 反向代理設定）
- [ ] 定期備份資料庫
- [ ] 定期更新 Docker 映像
- [ ] 監控服務日誌
- [ ] 設定資源限制（CPU、記憶體）

### AWS EC2 安全組設定
確保安全組允許以下端口：
- 22 (SSH)
- 5173 (前端)
- 5174 (管理後台)
- 5246 (API)
- 80 (HTTP，如使用反向代理)
- 443 (HTTPS，如使用反向代理)

## 🌐 進階部署選項

### Nginx 反向代理設定（支援 HTTPS）

```nginx
# /etc/nginx/sites-available/badminton-forum
server {
    listen 80;
    server_name your-domain.com;
    
    # 自動重定向到 HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name your-domain.com;
    
    # SSL 憑證（使用 Let's Encrypt）
    ssl_certificate /etc/letsencrypt/live/your-domain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/your-domain.com/privkey.pem;
    
    # 前端
    location / {
        proxy_pass http://localhost:5173;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
    
    # API
    location /api {
        proxy_pass http://localhost:5246;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
    
    # 管理後台
    location /admin {
        proxy_pass http://localhost:5174;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### SSL/HTTPS 設定（使用 Let's Encrypt）

```bash
# 安裝 Certbot
sudo apt update
sudo apt install certbot python3-certbot-nginx

# 取得 SSL 憑證
sudo certbot --nginx -d your-domain.com

# 自動更新憑證
sudo certbot renew --dry-run
```

### Docker 資源限制

在 `docker-compose.prod.yml` 中添加資源限制：
```yaml
services:
  api:
    deploy:
      resources:
        limits:
          cpus: '1'
          memory: 1G
        reservations:
          cpus: '0.5'
          memory: 512M
```

## 📚 版本資訊

### 當前穩定版本
- **Tag**: v1.0.0-ec2-stable
- **日期**: 2025-08-17
- **測試環境**: AWS EC2 (Ubuntu)
- **Docker Compose**: v2.39.1
- **Docker**: 28.3.3

### 版本特性
- ✅ 完整的前後端分離架構
- ✅ Docker Compose 生產環境配置
- ✅ 一鍵部署腳本
- ✅ 管理員後台
- ✅ Google OAuth 登入支援
- ✅ 戰術板功能
- ✅ 富文本編輯器

## 🆘 取得協助

如遇到問題，請檢查：
1. 本文檔的故障排除章節
2. 查看服務日誌：`docker compose -f docker-compose.prod.yml logs -f`
3. 檢查 `.env` 設定是否正確
4. 確認已使用 `--build` 參數重新建置

常見問題解答和更新請參考專案 GitHub Issues。