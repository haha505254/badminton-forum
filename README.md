# 羽毛球論壇 (Badminton Forum)

一個使用 ASP.NET Core 和 Vue.js 建立的現代化羽毛球討論論壇。

## 技術棧

### 後端
- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL 資料庫
- JWT 身份驗證
- Swagger API 文檔

### 前端
- Vue 3
- Vue Router 4
- Pinia (狀態管理)
- Axios (HTTP 客戶端)
- Vite (構建工具)

## 專案結構

```
badminton-forum/
├── BadmintonForum.API/          # ASP.NET Core Web API
│   ├── Controllers/             # API 控制器
│   ├── Models/                  # 資料模型
│   ├── Services/                # 業務邏輯服務
│   ├── Data/                    # 資料庫上下文
│   └── DTOs/                    # 資料傳輸物件
├── badminton-forum-vue/         # Vue.js 前端
│   ├── src/
│   │   ├── components/          # Vue 組件
│   │   ├── views/               # 頁面視圖
│   │   ├── router/              # 路由配置
│   │   ├── stores/              # Pinia 狀態管理
│   │   └── api/                 # API 客戶端
│   └── package.json
└── README.md
```

## 功能特色

- 🔐 **用戶系統**：註冊、登入、個人資料管理
- 📝 **論壇功能**：發表文章、回覆留言、點讚功能
- 🏷️ **分類系統**：技術討論、裝備推薦、活動公告、球友交流
- 🔍 **搜索功能**：全文搜索、標籤篩選
- 📱 **響應式設計**：支援各種設備瀏覽

## 開始使用

### 先決條件

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (建議 v18+)
- [PostgreSQL](https://www.postgresql.org/) 資料庫

### 設置後端

1. 進入 API 目錄：
   ```bash
   cd BadmintonForum.API
   ```

2. 還原 NuGet 套件：
   ```bash
   dotnet restore
   ```

3. 配置資料庫連接字串：
   編輯 `appsettings.json`，更新 PostgreSQL 連接資訊：
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=BadmintonForumDb;Username=postgres;Password=yourREMOVED"
   }
   ```

4. 創建資料庫和執行遷移：
   ```bash
   dotnet ef database update
   ```

5. 運行 API：
   ```bash
   dotnet run
   ```

   API 將在 `https://localhost:5001` 和 `http://localhost:5000` 上運行。

### 設置前端

1. 進入前端目錄：
   ```bash
   cd badminton-forum-vue
   ```

2. 安裝依賴：
   ```bash
   npm install
   ```

3. 配置 API 端點（如需要）：
   創建 `.env.local` 文件：
   ```
   VITE_API_URL=http://localhost:5000/api
   ```

4. 運行開發服務器：
   ```bash
   npm run dev
   ```

   前端將在 `http://localhost:5173` 上運行。

## API 文檔

當 API 運行時，可以訪問 Swagger UI 查看完整的 API 文檔：
- http://localhost:5000/swagger

## 資料庫架構

### 主要資料表
- **Users**: 用戶資訊
- **Categories**: 論壇版塊分類
- **Posts**: 文章
- **Replies**: 回覆

## 開發指南

### 添加新的 API 端點

1. 在 `Controllers` 目錄中創建新的控制器
2. 使用 `[ApiController]` 和 `[Route]` 屬性
3. 實現必要的 CRUD 操作

### 添加新的前端頁面

1. 在 `views` 目錄中創建新的 Vue 組件
2. 在 `router/index.js` 中添加路由
3. 如需狀態管理，在 `stores` 中創建新的 store

## 部署

### 後端部署
1. 發布應用程式：
   ```bash
   dotnet publish -c Release
   ```

2. 配置生產環境的資料庫連接和 JWT 密鑰

### 前端部署
1. 構建生產版本：
   ```bash
   npm run build
   ```

2. 將 `dist` 目錄部署到 Web 服務器

## 貢獻

歡迎提交 Pull Request 或開啟 Issue！

## 授權

MIT License