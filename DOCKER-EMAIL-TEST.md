# Docker 環境忘記密碼功能測試指南

## 啟動服務

```bash
# 在專案根目錄執行
docker-compose up
```

## 測試步驟

### 1. 確認服務運行正常
- 前端：http://localhost:5173
- 後端 API：http://localhost:5246/swagger
- 資料庫管理（如果啟用）：http://localhost:8080

### 2. 測試忘記密碼功能

1. **註冊測試帳號**
   - 前往 http://localhost:5173/register
   - 註冊一個測試帳號（例如：test@docker.com）

2. **使用忘記密碼**
   - 前往 http://localhost:5173/login
   - 點擊「忘記密碼？」
   - 輸入剛註冊的 Email

3. **查看重置連結**
   - 查看 Docker 容器的日誌：
   ```bash
   # 查看 API 容器的日誌
   docker logs badminton-forum-api
   
   # 或即時追蹤日誌
   docker logs -f badminton-forum-api
   ```

4. **找到重置連結**
   您會在日誌中看到：
   ```
   ┌─────────────────────────────────────────────────────────────┐
   │                    🔐 密碼重置連結                          │
   ├─────────────────────────────────────────────────────────────┤
   │ 收件人: test@docker.com                                     │
   │ 使用者: test@docker.com                                     │
   ├─────────────────────────────────────────────────────────────┤
   │ 請複製下方連結到瀏覽器:                                    │
   │                                                             │
   │ http://localhost:5173/reset-password?token=xxxxx...        │
   │                                                             │
   │ ⏰ 此連結 24 小時後失效                                    │
   └─────────────────────────────────────────────────────────────┘
   ```

5. **重置密碼**
   - 複製連結到瀏覽器
   - 設定新密碼
   - 使用新密碼登入測試

## 常用 Docker 命令

```bash
# 查看所有容器狀態
docker-compose ps

# 查看 API 日誌（含重置連結）
docker logs badminton-forum-api

# 即時追蹤 API 日誌
docker logs -f badminton-forum-api

# 只看最新的 50 行日誌
docker logs --tail 50 badminton-forum-api

# 重啟 API 容器
docker-compose restart api

# 完全重建並啟動
docker-compose up --build
```

## 注意事項

1. **連結 URL**：確保連結中的網址是 `http://localhost:5173`，不是容器內部網址

2. **日誌太多找不到**：可以清空日誌後重試
   ```bash
   docker-compose restart api
   docker logs -f badminton-forum-api
   ```

3. **生產環境**：記得關閉 `Email__UseConsoleEmail`，改用真實的 Email 服務

## 環境變數設定

如果需要自訂設定，建立 `.env` 檔案：

```env
# Email 設定
EMAIL_BASE_URL=http://localhost:5173
EMAIL_USE_CONSOLE=true

# 如果未來要用真實 Email
EMAIL_SMTP_HOST=smtp.gmail.com
EMAIL_SMTP_PORT=587
EMAIL_SMTP_USERNAME=your-email@gmail.com
EMAIL_SMTP_PASSWORD=your-app-password
```