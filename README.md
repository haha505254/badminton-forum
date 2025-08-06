# Badminton Forum 羽球論壇

一個使用 ASP.NET Core Web API + Vue.js 建立的羽球討論論壇。

## 功能特色

- 🔐 會員註冊/登入系統（含忘記密碼功能）
- 📝 發表文章與回覆
- 🏷️ 分類瀏覽（技術討論、裝備評測、比賽資訊、找球友）
- 👤 個人資料管理（含頭像上傳）
- 🛡️ 管理員後台
- 🔍 搜尋功能
- 🔑 Google OAuth 登入（選用）

## 技術架構

- **後端**: ASP.NET Core 8.0 Web API
- **前端**: Vue 3 + Vite + Tailwind CSS
- **資料庫**: MariaDB 11+
- **認證**: JWT Token + Google OAuth 2.0
- **ORM**: Entity Framework Core
- **狀態管理**: Pinia
- **容器化**: Docker & Docker Compose

## 🚀 快速開始

### 前置需求
- Docker 和 Docker Compose
- Git

### 安裝步驟

```bash
# 1. Clone 專案
git clone https://github.com/haha505254/badminton-forum.git
cd badminton-forum

# 2. 設定環境變數
cp .env.example .env
cp badminton-forum-vue/.env.development.example badminton-forum-vue/.env.development

# 3. (選用) 編輯 .env 檔案
# - 修改資料庫密碼（生產環境必須更改）
# - 設定 JWT Secret（生產環境必須更改）
# - 設定 Google OAuth Client ID（如需要 Google 登入功能）
# nano .env

# 4. 啟動所有服務
docker-compose up -d

# 5. 查看服務狀態
docker-compose ps
```

### 快速設定腳本（替代方案）

```bash
# 使用自動設定腳本
./scripts/quick-setup.sh

# 然後啟動服務
docker-compose up -d
```

稍等片刻後，即可訪問：
- 🌐 **前端**: http://localhost:5173
- 🔧 **API**: http://localhost:5246
- 📚 **API 文檔**: http://localhost:5246/swagger

就這麼簡單！🎉

### 停止服務

```bash
# 停止所有服務
docker-compose down

# 停止並清除資料（包含資料庫）
docker-compose down -v
```

## 🔧 Docker 環境管理

### 查看日誌

```bash
# 查看所有服務日誌
docker-compose logs -f

# 查看特定服務日誌
docker-compose logs -f api
docker-compose logs -f web
docker-compose logs -f db
```

### 執行資料庫遷移

```bash
# 在 Docker 容器中執行遷移
docker-compose exec api dotnet ef database update
```

### 進入容器內部

```bash
# 進入 API 容器
docker-compose exec api bash

# 進入前端容器
docker-compose exec web sh

# 連接到資料庫
docker-compose exec db mariadb -u badmintonuser -p badmintonforumdb
```

### 重新建置服務

```bash
# 重建所有服務
docker-compose up -d --build

# 重建特定服務
docker-compose up -d --build web
```

## 📝 預設帳號

系統預設沒有管理員帳號，註冊後可透過以下方式設定管理員：

```bash
# 將指定用戶設為管理員（替換 email@example.com）
docker-compose exec db mariadb -u badmintonuser -pBadmintonPass123 badmintonforumdb \
  -e "UPDATE Users SET IsAdmin = true WHERE Email = 'email@example.com';"
```

## 🛠️ 環境變數設定

專案使用 `.env` 檔案管理設定。請參考 `.env.example` 檔案：

### 必要設定
- **資料庫密碼**: 生產環境必須更改預設密碼
- **JWT Secret**: 生產環境必須使用強密碼（至少 32 字元）

### 選用設定
- **Google OAuth**: 設定 Client ID 以啟用 Google 登入
- **Email 服務**: 生產環境設定 SMTP 

詳細說明請查看 `.env.example` 檔案中的註解。

## 📂 專案結構

```
badminton-forum/
├── BadmintonForum.API/        # ASP.NET Core Web API
│   ├── Controllers/           # API 控制器
│   ├── Models/               # 資料模型
│   ├── DTOs/                 # 資料傳輸物件
│   ├── Services/             # 商業邏輯
│   ├── Migrations/           # EF Core 遷移
│   └── Dockerfile            # API Docker 設定
├── badminton-forum-vue/       # Vue.js 前端
│   ├── src/
│   │   ├── views/           # 頁面元件
│   │   ├── components/      # 共用元件
│   │   ├── api/            # API 呼叫
│   │   ├── stores/         # Pinia 狀態管理
│   │   └── router/         # 路由設定
│   └── Dockerfile          # 前端 Docker 設定
├── docker-compose.yml      # Docker Compose 設定
├── .env.example           # 環境變數範例
└── README.md             # 本文件
```

## 🐛 疑難排解

### Docker 相關問題

**問題：容器無法啟動**
```bash
# 查看詳細錯誤訊息
docker-compose logs

# 重新建置並啟動
docker-compose down
docker-compose up -d --build
```

**問題：資料庫連線失敗**
```bash
# 確認資料庫容器正在運行
docker-compose ps db

# 檢查資料庫日誌
docker-compose logs db
```

**問題：前端無法連接 API**
1. 確認 API 容器正在運行：`docker-compose ps api`
2. 檢查瀏覽器的 Console 錯誤
3. 確認 CORS 設定正確

### WSL/Windows 相關問題

如果在 WSL 上遇到 Docker 認證問題：
```bash
# 編輯 Docker 設定
nano ~/.docker/config.json

# 移除 "credsStore": "desktop.exe" 這一行
```

## 💻 本地開發（進階）

如果你想要在本地環境開發，而非使用 Docker：

<details>
<summary>點擊展開本地安裝步驟</summary>

### 前置需求

1. .NET 8.0 SDK
2. Node.js 18+
3. PostgreSQL 16+

### 安裝步驟

1. **設定資料庫**
   ```bash
   sudo -u postgres psql
   CREATE DATABASE badmintonforumdb;
   CREATE USER badmintonuser WITH ENCRYPTED PASSWORD 'your-password';
   GRANT ALL PRIVILEGES ON DATABASE badmintonforumdb TO badmintonuser;
   \c badmintonforumdb
   GRANT ALL ON SCHEMA public TO badmintonuser;
   \q
   ```

2. **設定後端**
   ```bash
   cd BadmintonForum.API
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=badmintonforumdb;Username=badmintonuser;Password=your-password"
   dotnet user-secrets set "JwtSettings:Secret" "your-secret-key-at-least-32-chars"
   dotnet ef database update
   dotnet run
   ```

3. **設定前端**
   ```bash
   cd badminton-forum-vue
   npm install
   npm run dev
   ```

</details>

## 🚀 生產環境部署

### 使用 Docker Compose

1. 修改 `.env` 檔案中的敏感資訊
2. 使用 `docker-compose.prod.yml`（如果有的話）
3. 設定反向代理（Nginx/Caddy）
4. 設定 SSL 憑證

### 手動部署

請參考 [部署文檔](./docs/deployment.md)（如果有的話）

## 🔐 安全注意事項

1. **生產環境務必更改所有預設密碼**
2. 使用環境變數管理敏感資訊
3. 定期更新依賴套件
4. 啟用 HTTPS
5. 設定適當的 CORS 政策

## 📞 需要幫助？

如果遇到問題：
1. 查看 [Issues](https://github.com/haha505254/badminton-forum/issues)
2. 查看服務日誌：`docker-compose logs`
3. 開新的 Issue 詢問

## 📄 授權

本專案採用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 檔案

---

💡 **提示**：使用 Docker Compose 是最快速簡單的開始方式！