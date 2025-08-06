# Google OAuth 設定指南

本文件說明如何設定 Google OAuth 以啟用 Google 登入功能。

## 1. 建立 Google Cloud Platform 專案

1. 前往 [Google Cloud Console](https://console.cloud.google.com/)
2. 建立新專案或選擇現有專案
3. 確保專案已啟用結算功能（OAuth 2.0 本身免費）

## 2. 啟用必要的 API

1. 在左側選單中，選擇「API 和服務」>「已啟用的 API」
2. 點擊「+ 啟用 API 和服務」
3. 搜尋並啟用：
   - Google+ API（或 Google Identity Toolkit API）
   - People API

## 3. 建立 OAuth 2.0 憑證

1. 前往「API 和服務」>「憑證」
2. 點擊「+ 建立憑證」>「OAuth 用戶端 ID」
3. 如果尚未設定同意畫面，請先設定：
   - 選擇「外部」使用者類型
   - 填寫應用程式名稱：「羽球論壇」
   - 新增支援電子郵件
   - 新增授權網域

4. 建立 OAuth 用戶端 ID：
   - 應用程式類型：「網頁應用程式」
   - 名稱：「羽球論壇 Web Client」
   - 授權的 JavaScript 來源：
     ```
     http://localhost:5173
     http://localhost:5246
     https://your-production-domain.com
     ```
   - 授權的重新導向 URI（選填）：
     ```
     http://localhost:5173/callback
     https://your-production-domain.com/callback
     ```

5. 記下產生的：
   - 用戶端 ID（Client ID）
   - 用戶端密鑰（Client Secret）- 後端不需要，但建議保存

## 4. 設定應用程式

### 後端設定（ASP.NET Core）

1. **開發環境** - 使用 User Secrets：
   ```bash
   cd BadmintonForum.API
   dotnet user-secrets set "GoogleAuth:ClientId" "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com"
   ```

2. **生產環境** - 設定環境變數：
   ```bash
   export GoogleAuth__ClientId="YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com"
   ```

3. **Docker** - 在 docker-compose.yml 中：
   ```yaml
   environment:
     - GoogleAuth__ClientId=${GOOGLE_CLIENT_ID}
   ```

### 前端設定（Vue 3）

1. **開發環境** - 編輯 `.env.development`：
   ```env
   VITE_GOOGLE_CLIENT_ID=YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com
   ```

2. **生產環境** - 編輯 `.env.production`：
   ```env
   VITE_GOOGLE_CLIENT_ID=YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com
   ```

## 5. 測試 Google OAuth

### 本地測試步驟

1. 啟動後端服務：
   ```bash
   cd BadmintonForum.API
   dotnet run
   ```

2. 啟動前端服務：
   ```bash
   cd badminton-forum-vue
   npm run dev
   ```

3. 開啟瀏覽器訪問：http://localhost:5173

4. 測試流程：
   - 點擊登入頁面的「使用 Google 登入」按鈕
   - 選擇或輸入 Google 帳號
   - 授權應用程式存取基本資料
   - 確認成功登入並導向首頁

### 測試案例

1. **新使用者註冊**
   - 使用從未註冊過的 Google 帳號
   - 確認自動建立新帳號
   - 確認使用者名稱從 email 生成

2. **現有使用者登入**
   - 使用已註冊的 Google 帳號
   - 確認正確識別並登入

3. **帳號綁定**
   - 先使用 email 註冊普通帳號
   - 使用相同 email 的 Google 帳號登入
   - 確認帳號正確綁定

4. **錯誤處理**
   - 測試無效的 ID Token
   - 測試網路錯誤情況
   - 確認錯誤訊息正確顯示

## 6. 疑難排解

### 常見問題

1. **「無效的 Google 認證令牌」錯誤**
   - 確認 Client ID 設定正確
   - 確認前後端使用相同的 Client ID
   - 檢查 Google Cloud Console 中的授權網域設定

2. **CORS 錯誤**
   - 確認後端 CORS 設定包含前端網址
   - 檢查 Program.cs 中的 CORS 政策

3. **Google Sign-In SDK 未載入**
   - 檢查網路連線
   - 確認未被廣告攔截器阻擋
   - 查看瀏覽器控制台錯誤訊息

4. **使用者資料未正確儲存**
   - 檢查資料庫連線
   - 確認 User 模型已正確遷移
   - 查看後端日誌錯誤訊息

## 7. 安全性建議

1. **保護 Client ID**
   - 雖然 Client ID 是公開的，但不要在程式碼中硬編碼
   - 使用環境變數管理

2. **驗證 ID Token**
   - 後端必須驗證 Google ID Token 的真實性
   - 不要信任前端傳來的使用者資訊

3. **HTTPS 要求**
   - 生產環境必須使用 HTTPS
   - Google OAuth 在生產環境不支援 HTTP

4. **定期更新**
   - 定期更新 Google.Apis.Auth NuGet 套件
   - 關注 Google Identity Services 的更新

## 8. 參考資源

- [Google Identity Services 文件](https://developers.google.com/identity/gsi/web)
- [Google Cloud Console](https://console.cloud.google.com/)
- [OAuth 2.0 最佳實踐](https://developers.google.com/identity/protocols/oauth2/web-server)
- [JWT 驗證文件](https://developers.google.com/identity/sign-in/web/backend-auth)