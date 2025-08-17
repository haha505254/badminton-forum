# Badminton Forum 羽球論壇

一個使用 ASP.NET Core Web API + Vue.js 建立的羽球討論論壇。

## 功能特色

### 核心功能
- 🔐 **會員系統**：註冊/登入、忘記密碼、Google OAuth 2.0
- 📝 **發文系統**：富文本編輯器 (TipTap)、分類管理、置頂/鎖定功能
- 💬 **回覆系統**：巢狀回覆、引用回覆、軟刪除
- 🏷️ **分類瀏覽**：綜合討論、技術交流、裝備討論、賽事專區、地區球友會
- 👤 **個人資料**：頭像上傳、個人簡介、發文記錄
- 🔍 **搜尋功能**：全文搜尋、分類篩選、作者篩選

### 進階功能
- 🏸 **戰術板編輯器**：互動式羽球場地圖表 (Konva.js)
  - 單打/雙打模式切換
  - 球員位置標記
  - 球路軌跡繪製
  - 移動路線箭頭
  - 文字註解功能
- 👍 **社交互動**：貼文按讚、熱門排序
- 📊 **管理後台**：獨立管理介面 (Vuestic Admin)
  - 即時統計儀表板
  - 用戶管理（啟用/停用、權限設定）
  - 貼文管理（置頂、鎖定、軟刪除）
  - 分類管理（新增、編輯、排序）
  - 回覆管理（批量操作、進階搜尋）

## 技術架構

- **後端**: ASP.NET Core 8.0 Web API
- **主前端**: Vue 3 + Vite + Tailwind CSS
- **管理後台**: Vuestic Admin + Vue 3 + TypeScript
- **資料庫**: MariaDB 11+
- **認證**: JWT Token + Google OAuth 2.0
- **ORM**: Entity Framework Core
- **狀態管理**: Pinia
- **富文本編輯**: TipTap
- **圖表繪製**: Konva.js + Vue-Konva
- **容器化**: Docker & Docker Compose

## 🚀 快速開始（超簡單！）

### 前置需求
- Docker 和 Docker Compose
- Git

### 安裝步驟

```bash
# 1. Clone 專案
git clone https://github.com/haha505254/badminton-forum.git
cd badminton-forum

# 2. 複製環境設定
cp .env.defaults .env
# 注意：.env 檔案包含所有預設值，可直接使用
# 生產環境請修改敏感資訊（密碼、JWT密鑰等）

# 3. 啟動所有服務
docker-compose up -d
```

就這麼簡單！

稍等片刻後，即可訪問：
- 🌐 **主前端**: http://localhost:5173
- 🛡️ **管理後台**: http://localhost:5174
- 🔧 **API**: http://localhost:5246
- 📚 **API 文檔**: http://localhost:5246/swagger
- 🗄️ **資料庫管理** (選用): http://localhost:8080 (需執行 `docker-compose --profile tools up`)

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

## 🛡️ 管理後台系統

本專案包含獨立的管理後台系統，提供完整的論壇管理功能：

### 存取管理後台
- **URL**: http://localhost:5174
- **預設管理員帳號**: 
  - Email: `admin@badminton-forum.com`
  - 密碼: `Admin123456!`

### 管理功能
- **儀表板**: 即時統計數據、活動圖表、分類分佈
- **用戶管理**: 啟用/停用帳號、授予/撤銷管理員權限
- **貼文管理**: 置頂、鎖定、軟刪除貼文
- **分類管理**: 新增、編輯、排序分類
- **回覆管理**: 批量操作、進階搜尋、巢狀回覆管理

## 📝 預設帳號

### 預設管理員帳號
系統會在首次啟動時自動創建管理員帳號：
- **Email**: admin@badminton-forum.com
- **密碼**: Admin123456!

生產環境部署前，請修改 `.env` 中的管理員設定：
```bash
DEFAULT_ADMIN_EMAIL=your-email@domain.com
DEFAULT_ADMIN_PASSWORD=YourSecurePassword!
```

一般用戶可透過註冊頁面建立。

### 設定新管理員
若需要將其他用戶設為管理員：

```bash
# 將指定用戶設為管理員（替換 email@example.com）
docker-compose exec db mariadb -u badmintonuser -pBadmintonPass123 badmintonforumdb \
  -e "UPDATE Users SET IsAdmin = true WHERE Email = 'email@example.com';"
```

## 🛠️ 環境變數設定

專案使用 `.env` 檔案管理設定。預設值請參考 `.env.defaults` 檔案：

### 必要設定
```bash
# 資料庫設定
MARIADB_DATABASE=badmintonforumdb
MARIADB_USER=badmintonuser
MARIADB_PASSWORD=BadmintonPass123    # ⚠️ 生產環境必須更改
MARIADB_ROOT_PASSWORD=rootpass123    # ⚠️ 生產環境必須更改

# JWT 設定
JWT_SECRET=ThisIsAVerySecretKeyForJWTTokenGenerationPleaseChangeInProduction  # ⚠️ 生產環境必須更改
JWT_EXPIRATION_DAYS=7
```

### 選用設定
```bash
# Google OAuth（選用）
GOOGLE_CLIENT_ID=YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com

# 管理後台設定
ADMIN_PORT=5174
ADMIN_APP_NAME=羽球論壇管理後台

# Adminer 資料庫管理工具（選用）
ADMINER_PORT=8080
```

詳細說明請查看 `.env.defaults` 檔案中的註解。

## 📂 專案結構

```
badminton-forum/
├── BadmintonForum.API/        # ASP.NET Core Web API
│   ├── Controllers/           # API 控制器
│   ├── Models/               # 資料模型（含 PostLike 按讚功能）
│   ├── DTOs/                 # 資料傳輸物件
│   ├── Services/             # 商業邏輯（JWT、Email、Google OAuth）
│   ├── Data/                 # DbContext 設定
│   ├── Migrations/           # EF Core 遷移
│   ├── migrations-sql/       # Idempotent SQL 腳本
│   └── Dockerfile            # API Docker 設定
├── badminton-forum-vue/       # Vue.js 主前端
│   ├── src/
│   │   ├── views/           # 頁面元件
│   │   ├── components/      # 共用元件
│   │   │   ├── BadmintonCourtDiagram.vue  # 戰術板編輯器
│   │   │   ├── BadmintonCourtViewer.vue   # 戰術板檢視器
│   │   │   ├── ReplyThread.vue            # 巢狀回覆元件
│   │   │   └── RichTextEditor.vue         # TipTap 編輯器
│   │   ├── api/            # API 呼叫模組
│   │   ├── stores/         # Pinia 狀態管理
│   │   └── router/         # 路由設定
│   └── Dockerfile          # 前端 Docker 設定
├── badminton-forum-admin/     # Vuestic Admin 管理後台
│   ├── src/
│   │   ├── pages/          # 管理頁面
│   │   │   ├── admin/dashboard/    # 儀表板
│   │   │   ├── users/              # 用戶管理
│   │   │   ├── posts/              # 貼文管理
│   │   │   ├── categories/         # 分類管理
│   │   │   └── replies/            # 回覆管理
│   │   ├── components/     # Vuestic UI 元件
│   │   ├── api/           # API 客戶端模組
│   │   ├── stores/        # Pinia 狀態管理
│   │   └── router/        # 路由與守衛
│   └── Dockerfile         # 管理後台 Docker 設定
├── docs/                  # 專案文檔
│   ├── GOOGLE_OAUTH_SETUP.md     # Google OAuth 設定指南
│   ├── EMAIL-SETUP.md             # Email 服務設定
│   ├── DEPLOYMENT-PRODUCTION.md  # 生產部署指南
│   └── CICD_SETUP.md             # CI/CD 設定
├── docker-compose.yml     # Docker Compose 設定
├── .env.defaults         # 環境變數預設值
├── CLAUDE.md            # AI 開發助手指引
└── README.md           # 本文件
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

## 🔧 開發注意事項

### 資料庫遷移規則（重要！）

**絕對不要這樣做：**
1. ❌ **絕不直接修改 Docker 容器中的資料庫**
2. ❌ **絕不使用原始 SQL ALTER TABLE 語句**
3. ❌ **絕不跳過 Migration 工作流程**

**正確做法：**
```bash
# 1. 修改 Model 類別 (Models/*.cs)
# 2. 建立遷移
dotnet ef migrations add [DescriptiveName]

# 3. 生成 idempotent SQL腳本
dotnet ef migrations script --idempotent -o migrations-sql/test.sql

# 4. 測試執行 SQL
docker-compose exec api dotnet ef database update

# 5. 如果成功，更新完整 SQL
dotnet ef migrations script --idempotent -o migrations-sql/all-existing.sql
```

### Docker 開發工作流程

1. **假設 Docker Compose 已在運行**
2. 本地編輯程式碼（熱重載會自動套用變更）
3. .NET 命令：使用 `docker-compose exec api dotnet [command]`
4. npm 命令：使用 `docker-compose exec web npm [command]` 或 `docker-compose exec admin npm [command]`

## 💻 本地開發（進階）

如果你想要在本地環境開發，而非使用 Docker：

<details>
<summary>點擊展開本地安裝步驟</summary>

### 前置需求

1. .NET 8.0 SDK
2. Node.js 18+
3. MariaDB 11+

### 安裝步驟

1. **設定資料庫**
   ```bash
   sudo mysql -u root -p
   CREATE DATABASE badmintonforumdb;
   CREATE USER 'badmintonuser'@'localhost' IDENTIFIED BY 'your-password';
   GRANT ALL PRIVILEGES ON badmintonforumdb.* TO 'badmintonuser'@'localhost';
   FLUSH PRIVILEGES;
   EXIT;
   ```

2. **設定後端**
   ```bash
   cd BadmintonForum.API
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=badmintonforumdb;User=badmintonuser;Password=your-password"
   dotnet user-secrets set "JwtSettings:Secret" "your-secret-key-at-least-32-chars"
   dotnet ef database update
   dotnet run
   ```

3. **設定主前端**
   ```bash
   cd badminton-forum-vue
   npm install
   npm run dev
   ```

4. **設定管理後台**
   ```bash
   cd badminton-forum-admin
   npm install
   npm run dev
   ```

</details>

## 🧪 測試

- **單元測試**：使用 xUnit 進行 .NET 測試
- **E2E 測試**：目前已停用（Playwright 配置仍保留）
- **API 測試**：使用 Swagger UI 或 `BadmintonForum.API.http` 檔案

```bash
# 執行後端測試
dotnet test

# API 測試
# 訪問 http://localhost:5246/swagger
```

## 🚀 CI/CD 流程

### GitHub Actions 工作流程

1. **CI 流程** (`ci-cd.yml`)：所有分支都會執行
   - .NET 格式檢查 (`dotnet format --verify-no-changes`)
   - API 測試
   - 前端建置驗證
   - Docker 建置驗證

2. **安全掃描** (`security.yml`)：每週 CodeQL 安全分析

3. **測試流程** (`test-cicd.yml`)：簡單的 CI/CD 測試工作流程

## 🚀 生產環境部署

### 使用 Docker Compose

1. 修改 `.env` 檔案中的敏感資訊
2. 使用 `docker-compose.prod.yml`
3. 設定反向代理（Nginx/Caddy）
4. 設定 SSL 憑證

### 詳細部署指南

- [生產部署指南](./docs/DEPLOYMENT-PRODUCTION.md)
- [Google OAuth 設定](./docs/GOOGLE_OAUTH_SETUP.md)
- [Email 服務設定](./docs/EMAIL-SETUP.md)
- [CI/CD 設定](./docs/CICD_SETUP.md)

## 📍 開發環境 URL

完整服務存取位址：

| 服務 | URL | 說明 |
|------|-----|------|
| 主前端 | http://localhost:5173 | 論壇主要介面 |
| 管理後台 | http://localhost:5174 | 管理員控制台 |
| API | http://localhost:5246 | 後端 API |
| Swagger | http://localhost:5246/swagger | API 文檔與測試 |
| Adminer | http://localhost:8080 | 資料庫管理工具（需 `--profile tools`） |
| MariaDB | localhost:3306 | 資料庫服務 |

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