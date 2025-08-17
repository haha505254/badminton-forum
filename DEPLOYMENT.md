# 部署指南

## 快速部署

### 開發環境（本地）

```bash
# 1. 複製環境設定
cp .env.defaults .env

# 2. 啟動服務（自動使用開發配置）
docker-compose up
```

### 生產環境（伺服器）

```bash
# 1. 複製環境設定
cp .env.defaults .env

# 2. 修改 .env 設定你的伺服器 IP
# 編輯 .env 檔案，修改以下設定：
# SERVER_IP=你的伺服器IP (例如: 56.155.146.48)
# ALLOWED_ORIGINS=http://你的伺服器IP:5173,http://你的伺服器IP:5174
# VITE_API_URL=http://你的伺服器IP:5246/api

# 3. 使用生產配置啟動
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# 4. 檢查服務狀態
docker-compose ps
```

## 環境差異

### 開發環境
- 使用 `docker-compose.yml` 單獨運行
- 包含熱重載（Hot Reload）
- 程式碼掛載為 volumes
- 資料庫端口暴露（3306）
- 包含 Adminer 資料庫管理介面

### 生產環境
- 使用 `docker-compose.yml` + `docker-compose.prod.yml`
- 使用最佳化的生產建置
- 無程式碼掛載（使用映像內的程式碼）
- 資料庫端口不對外暴露
- 所有服務設定 `restart: always`
- 移除 Adminer

## 重要環境變數

| 變數名 | 說明 | 範例 |
|--------|------|------|
| `SERVER_IP` | 伺服器 IP 或域名 | `56.155.146.48` |
| `ALLOWED_ORIGINS` | 允許的 CORS 來源 | `http://56.155.146.48:5173,http://56.155.146.48:5174` |
| `VITE_API_URL` | 前端連接 API 的 URL | `http://56.155.146.48:5246/api` |
| `JWT_SECRET` | JWT 加密金鑰（生產環境必須更改） | 使用強隨機字串 |
| `MARIADB_PASSWORD` | 資料庫密碼（生產環境必須更改） | 使用強密碼 |

## 常用命令

```bash
# 查看日誌
docker-compose logs -f [service-name]

# 重啟特定服務
docker-compose restart [service-name]

# 停止所有服務
docker-compose down

# 停止並清除資料（注意：會刪除資料庫）
docker-compose down -v

# 重新建置映像
docker-compose build --no-cache

# 進入容器
docker-compose exec api bash
docker-compose exec web sh
docker-compose exec admin sh
```

## 故障排除

### CORS 錯誤
確認 `ALLOWED_ORIGINS` 環境變數包含正確的前端 URL。

### 資料庫連線失敗
檢查資料庫是否健康：
```bash
docker-compose exec db mariadb -u badmintonuser -p -e "SELECT 1"
```

### API 建置失敗
清除並重新建置：
```bash
docker-compose down
docker-compose build --no-cache api
docker-compose up -d
```

## 預設管理員帳號

系統會在首次啟動時自動創建管理員帳號。

### 開發環境預設值
- **Email**: admin@badminton-forum.com
- **Password**: Admin123456!

### 生產環境設定
**重要**：生產環境部署前必須修改 `.env` 中的管理員帳號密碼：
```bash
DEFAULT_ADMIN_EMAIL=your-admin@domain.com
DEFAULT_ADMIN_PASSWORD=YourSecurePassword123!
```

首次登入後建議立即修改密碼。

### 管理員功能
管理員可以：
- 存取管理後台 (http://localhost:5174)
- 管理用戶（啟用/停用、授予管理員權限）
- 管理貼文（置頂、鎖定、刪除）
- 管理分類（新增、編輯、刪除）
- 管理回覆（軟刪除、批次刪除）

## 備份與還原

### 備份資料庫
```bash
docker-compose exec db mysqldump -u badmintonuser -p badmintonforumdb > backup.sql
```

### 還原資料庫
```bash
docker-compose exec -T db mysql -u badmintonuser -p badmintonforumdb < backup.sql
```

## 進階部署選項

### Nginx 反向代理設定

如果你想使用 Nginx 作為反向代理（支援 HTTPS）：

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

### 安全性建議

生產環境部署時，請確保：
- ✅ 更改所有預設密碼（資料庫、JWT、管理員）
- ✅ 使用 HTTPS/SSL 加密連線
- ✅ 設定防火牆規則，只開放必要端口
- ✅ 定期備份資料庫
- ✅ 定期更新 Docker 映像
- ✅ 監控服務狀態和日誌