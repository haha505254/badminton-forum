# Claude 使用指南

## 快速開始

### 啟動專案
```bash
# 終端 1 - 啟動後端
cd BadmintonForum.API
dotnet run

# 終端 2 - 啟動前端
cd badminton-forum-vue
npm run dev
```

### 常用指令
- 執行資料庫遷移：`dotnet ef database update`
- 建立新的遷移：`dotnet ef migrations add <MigrationName>`
- 執行測試：`dotnet test`
- 建置專案：`dotnet build`

## 專案結構說明

### 後端 (BadmintonForum.API)
- `Controllers/` - API 控制器
- `Models/` - 資料模型 (User, Post, Reply, Category)
- `DTOs/` - 資料傳輸物件
- `Services/` - 商業邏輯服務 (如 JwtService)
- `Data/` - EF Core DbContext
- `Migrations/` - 資料庫遷移檔案

### 前端 (badminton-forum-vue)
- `src/views/` - 頁面元件
- `src/api/` - API 呼叫模組
- `src/stores/` - Pinia 狀態管理
- `src/router/` - Vue Router 設定

## 重要設定

### 資料庫連線
位於 `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=badmintonforumdb;Username=badmintonuser;Password=REMOVED"
}
```

### API 端點
- 後端：http://localhost:5246
- 前端：http://localhost:5173
- Swagger：http://localhost:5246/swagger

## 注意事項
1. 資料庫遷移時若遇到動態值問題，確保 Model 中不使用 `DateTime.UtcNow` 作為預設值
2. 前端 API 位址透過 `.env.development` 設定
3. Git 推送使用 Personal Access Token 認證