# 🐳 Docker 快速開始指南

只需要 **3 個步驟**就能開始開發！

## 前置需求

- 安裝 [Docker Desktop](https://www.docker.com/products/docker-desktop)
- 就這樣！不需要安裝 .NET、Node.js、PostgreSQL

## 🚀 快速開始

### 1️⃣ Clone 專案
```bash
git clone [專案網址]
cd badminton-forum
```

### 2️⃣ 設定環境變數
```bash
cp .env.docker.example .env
```

### 3️⃣ 啟動所有服務
```bash
docker-compose up
```

## ✅ 完成！

現在你可以訪問：
- 🌐 **前端**: http://localhost:5173
- 🔧 **API**: http://localhost:5246
- 📚 **API 文件**: http://localhost:5246/swagger
- 🗄️ **資料庫管理**: http://localhost:8080 (Adminer，選用)

## 📝 常用指令

### 基本操作
```bash
# 啟動所有服務（前景執行）
docker-compose up

# 啟動所有服務（背景執行）
docker-compose up -d

# 停止所有服務
docker-compose down

# 查看服務狀態
docker-compose ps

# 查看服務日誌
docker-compose logs -f [service-name]
```

### 資料庫操作
```bash
# 進入資料庫容器
docker-compose exec db psql -U badmintonuser -d badmintonforumdb

# 備份資料庫
docker-compose exec db pg_dump -U badmintonuser badmintonforumdb > backup.sql

# 還原資料庫
docker-compose exec -T db psql -U badmintonuser -d badmintonforumdb < backup.sql
```

### 開發操作
```bash
# 重新建置並啟動（當 Dockerfile 改變時）
docker-compose up --build

# 只重新建置特定服務
docker-compose build api
docker-compose build web

# 進入容器執行指令
docker-compose exec api bash
docker-compose exec web sh

# 執行 EF Core 遷移
docker-compose exec api dotnet ef migrations add [MigrationName]
docker-compose exec api dotnet ef database update
```

## 🔧 環境變數說明

編輯 `.env` 來自訂設定：

```env
# 資料庫密碼（重要！請更改）
POSTGRES_PASSWORD=你的安全密碼

# JWT 密鑰（重要！請更改）
JWT_SECRET=你的JWT密鑰至少32字元

# 連接埠設定
API_PORT=5246      # API 連接埠
WEB_PORT=5173      # 前端連接埠
DB_PORT=5432       # 資料庫連接埠
```

## 🛠️ 進階功能

### 使用 Adminer 管理資料庫
```bash
# 啟動包含 Adminer 的服務
docker-compose --profile tools up -d

# 訪問 http://localhost:8080
# 登入資訊：
# - 系統: PostgreSQL
# - 伺服器: db
# - 使用者: badmintonuser
# - 密碼: [你的密碼]
# - 資料庫: badmintonforumdb
```

### 生產環境部署
```bash
# 建置生產版本
docker-compose -f docker-compose.yml -f docker-compose.prod.yml build

# 啟動生產環境
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## 🐛 疑難排解

### 問題：連接埠已被占用
```bash
# 修改 .env 中的連接埠設定
API_PORT=5247
WEB_PORT=5174
```

### 問題：資料庫連線失敗
```bash
# 重新啟動資料庫
docker-compose restart db

# 檢查資料庫日誌
docker-compose logs db
```

### 問題：前端無法連接 API
```bash
# 確認 API 正在執行
docker-compose ps api

# 檢查 API 日誌
docker-compose logs api
```

### 清理所有資料（重新開始）
```bash
# 停止並移除所有容器、網路、資料卷
docker-compose down -v

# 重新啟動
docker-compose up
```

## 📂 Docker 相關檔案說明

```
badminton-forum/
├── docker-compose.yml         # 主要編排檔案
├── .env                      # 環境變數設定
├── .env.docker.example       # 環境變數範例
├── BadmintonForum.API/
│   ├── Dockerfile           # API 容器定義
│   └── docker-entrypoint.sh # API 啟動腳本
└── badminton-forum-vue/
    ├── Dockerfile           # 前端容器定義
    └── nginx.conf          # Nginx 設定（生產用）
```

## 💡 提示

1. **熱重載**：程式碼修改會自動重新編譯，不需要重啟容器
2. **資料持久化**：資料庫資料保存在 Docker volume 中
3. **開發工具**：容器內已包含 dotnet-ef 等開發工具
4. **跨平台**：在 Windows、Mac、Linux 上運作一致

---

🎉 **就是這麼簡單！** 現在你可以專注於寫程式，不用擔心環境問題了。