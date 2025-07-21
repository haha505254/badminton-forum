# API 整合測試指南

## 測試前準備

1. **確保資料庫正在運行**：
   ```bash
   sudo systemctl status postgresql
   ```

2. **運行後端 API**（終端 1）：
   ```bash
   cd BadmintonForum.API
   dotnet run
   ```
   API 應該在 http://localhost:5000 上運行

3. **運行前端應用**（終端 2）：
   ```bash
   cd badminton-forum-vue
   npm run dev
   ```
   前端應該在 http://localhost:5173 上運行

## 測試步驟

### 1. 測試分類列表
- 訪問 http://localhost:5173
- 點擊「版塊」
- 應該看到 4 個預設的論壇版塊

### 2. 測試用戶註冊
- 點擊右上角「註冊」
- 填寫表單：
  - 用戶名：testuser
  - 電子郵件：test@example.com
  - 密碼：password123
  - 確認密碼：password123
- 點擊「註冊」
- 成功後應該自動登入並跳轉到首頁

### 3. 測試發表文章
- 確保已登入
- 點擊「發表文章」
- 選擇版塊（如「技術討論」）
- 填寫標題和內容
- 點擊「發表文章」
- 成功後應該跳轉到新文章頁面

### 4. 測試回覆功能
- 在文章頁面底部
- 填寫回覆內容
- 點擊「發表」
- 應該看到新的回覆出現

### 5. 測試個人資料
- 點擊右上角的用戶名
- 應該看到個人資料頁面
- 顯示用戶信息和發表的文章

### 6. 測試帳戶設置
- 點擊「設置」
- 更新個人簡介
- 點擊「保存更改」
- 測試修改密碼功能

## 故障排除

### API 無法連接
1. 檢查 API 是否在運行
2. 檢查端口是否正確（5000）
3. 檢查 CORS 配置

### 資料庫錯誤
1. 確保 PostgreSQL 正在運行
2. 檢查連接字串是否正確
3. 確保資料庫已創建並執行了遷移

### 認證問題
1. 檢查 JWT 配置
2. 確保 token 正確保存在 localStorage
3. 檢查 API 請求頭是否包含 Authorization

## API 端點清單

- **認證**
  - POST /api/auth/register
  - POST /api/auth/login

- **分類**
  - GET /api/categories
  - GET /api/categories/{id}
  - GET /api/categories/{id}/posts

- **文章**
  - GET /api/posts
  - GET /api/posts/{id}
  - POST /api/posts
  - PUT /api/posts/{id}
  - DELETE /api/posts/{id}
  - POST /api/posts/{id}/like

- **回覆**
  - GET /api/posts/{postId}/replies
  - POST /api/posts/{postId}/replies
  - PUT /api/posts/{postId}/replies/{id}
  - DELETE /api/posts/{postId}/replies/{id}
  - POST /api/posts/{postId}/replies/{id}/like

- **個人資料**
  - GET /api/profile/{username}
  - GET /api/profile/{username}/posts
  - PUT /api/profile
  - POST /api/profile/change-password