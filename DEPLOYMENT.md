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

## 備份與還原

### 備份資料庫
```bash
docker-compose exec db mysqldump -u badmintonuser -p badmintonforumdb > backup.sql
```

### 還原資料庫
```bash
docker-compose exec -T db mysql -u badmintonuser -p badmintonforumdb < backup.sql
```