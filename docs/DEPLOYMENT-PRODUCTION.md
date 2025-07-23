# 🚀 生產環境部署指南

本指南說明如何將 Badminton Forum 部署到生產環境。

## 📋 前置需求

1. 安裝 Docker 和 Docker Compose
2. 準備一個域名（例如：badminton-forum.com）
3. SSL 憑證（可使用 Let's Encrypt）
4. SMTP 郵件服務（Gmail、SendGrid 等）

## 🔧 部署步驟

### 1. 準備生產環境設定

```bash
# Clone 專案
git clone https://github.com/haha505254/badminton-forum.git
cd badminton-forum

# 複製生產環境變數範本
cp .env.production.example .env.production

# 編輯生產環境變數
nano .env.production
```

### 2. 設定必要的環境變數

編輯 `.env.production` 檔案，設定以下重要變數：

```env
# 資料庫密碼（使用強密碼）
MARIADB_PASSWORD=your-very-strong-password
MARIADB_ROOT_PASSWORD=your-even-stronger-root-password

# JWT 密鑰（至少 32 字元）
JWT_SECRET=your-random-32-character-secret-key

# Email 設定
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-specific-password
EMAIL_FROM_ADDRESS=noreply@your-domain.com

# 網站 URL
VITE_API_URL=https://api.your-domain.com/api
FRONTEND_URL=https://your-domain.com
```

### 3. 啟動生產環境

```bash
# 使用生產環境配置啟動
docker-compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.production up -d

# 檢查服務狀態
docker-compose -f docker-compose.yml -f docker-compose.prod.yml ps

# 查看日誌
docker-compose -f docker-compose.yml -f docker-compose.prod.yml logs -f
```

### 4. 設定反向代理（Nginx）

如果您使用 Nginx 作為反向代理：

```nginx
# /etc/nginx/sites-available/badminton-forum
server {
    listen 80;
    server_name your-domain.com www.your-domain.com;
    
    # 重定向到 HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name your-domain.com www.your-domain.com;
    
    # SSL 設定
    ssl_certificate /etc/letsencrypt/live/your-domain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/your-domain.com/privkey.pem;
    
    # 前端
    location / {
        proxy_pass http://localhost:80;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

# API 子域名
server {
    listen 443 ssl http2;
    server_name api.your-domain.com;
    
    ssl_certificate /etc/letsencrypt/live/api.your-domain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api.your-domain.com/privkey.pem;
    
    location / {
        proxy_pass http://localhost:5246;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### 5. 資料庫初始化

首次部署時，確保資料庫遷移成功：

```bash
# 執行資料庫遷移
docker-compose -f docker-compose.yml -f docker-compose.prod.yml exec api dotnet ef database update

# 建立管理員帳號（註冊後執行）
docker-compose -f docker-compose.yml -f docker-compose.prod.yml exec db mariadb -u badmintonuser -p${MARIADB_PASSWORD} badmintonforumdb \
  -e "UPDATE Users SET IsAdmin = true WHERE Email = 'admin@your-domain.com';"
```

## 🔒 安全性檢查清單

- [ ] 更改所有預設密碼
- [ ] 設定強密碼（20+ 字元）
- [ ] 啟用 HTTPS/SSL
- [ ] 關閉不必要的埠
- [ ] 定期更新 Docker 映像
- [ ] 設定防火牆規則
- [ ] 啟用日誌監控
- [ ] 定期備份資料庫

## 🔧 維護操作

### 更新應用程式

```bash
# 拉取最新程式碼
git pull origin main

# 重新建置並部署
docker-compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.production up -d --build

# 執行資料庫遷移（如果有）
docker-compose -f docker-compose.yml -f docker-compose.prod.yml exec api dotnet ef database update
```

### 備份資料庫

```bash
# 建立備份
docker-compose -f docker-compose.yml -f docker-compose.prod.yml exec db \
  mariadb-dump -u badmintonuser -p${MARIADB_PASSWORD} badmintonforumdb > backup_$(date +%Y%m%d_%H%M%S).sql

# 還原備份
docker-compose -f docker-compose.yml -f docker-compose.prod.yml exec -T db \
  mariadb -u badmintonuser -p${MARIADB_PASSWORD} badmintonforumdb < backup_20240123_120000.sql
```

### 監控服務

```bash
# 查看資源使用情況
docker stats

# 查看特定服務日誌
docker-compose -f docker-compose.yml -f docker-compose.prod.yml logs -f api
docker-compose -f docker-compose.yml -f docker-compose.prod.yml logs -f web

# 健康檢查
curl https://api.your-domain.com/health
```

## 🚨 故障排除

### 問題：服務無法啟動

```bash
# 檢查詳細錯誤
docker-compose -f docker-compose.yml -f docker-compose.prod.yml logs

# 確認環境變數
docker-compose -f docker-compose.yml -f docker-compose.prod.yml config
```

### 問題：資料庫連線失敗

```bash
# 測試資料庫連線
docker-compose -f docker-compose.yml -f docker-compose.prod.yml exec api \
  mariadb -h db -u badmintonuser -p${MARIADB_PASSWORD} -e "SELECT 1"
```

### 問題：Email 無法發送

1. 確認 SMTP 設定正確
2. 檢查防火牆是否允許 SMTP 埠
3. 確認使用應用程式專用密碼（Gmail）

## 📊 效能優化

1. **啟用 CDN**：靜態資源使用 CloudFlare 或其他 CDN
2. **資料庫索引**：定期檢查並優化資料庫索引
3. **快取策略**：設定適當的 HTTP 快取標頭
4. **壓縮**：啟用 Gzip/Brotli 壓縮

## 🔄 零停機部署

使用滾動更新策略：

```bash
# 1. 建置新版本
docker-compose -f docker-compose.yml -f docker-compose.prod.yml build

# 2. 更新服務（一次一個）
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --no-deps api
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --no-deps web
```

## 📞 需要協助？

如果遇到問題：
1. 查看 [Issues](https://github.com/haha505254/badminton-forum/issues)
2. 檢查服務日誌
3. 確認所有環境變數都已正確設定

---

💡 **提醒**：生產環境部署需要謹慎，建議先在測試環境驗證所有步驟。