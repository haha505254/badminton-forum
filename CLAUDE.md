# CLAUDE.md

此檔案為 Claude Code (claude.ai/code) 在此專案中工作時提供指引。

## 文件語言政策

**本專案的所有文件和程式碼註解應使用繁體中文撰寫，以確保團隊溝通的一致性。**

## 專案概述

全端羽球論壇應用程式，採用雙前端架構：
- **後端**：ASP.NET Core 8.0 Web API 搭配 MariaDB
- **主前端**：Vue 3 SPA 搭配 Vite（公開論壇）
- **管理後台**：Vuestic Admin 搭配 Vue 3 + TypeScript（管理儀表板）
- **架構**：RESTful API 搭配 JWT 身份驗證 + Google OAuth 2.0

## 🚨 重要：部署準備

**重要提醒：所有本地開發必須確保能使用 deploy.sh 腳本順利部署**

### 任何程式碼更改前
**務必確認您的更改不會破壞生產環境部署：**
1. 檢查 `DEPLOYMENT.md` 取得完整部署指南
2. 確保所有新的環境變數都已加入 `.env.defaults`
3. 考慮您的更改將如何影響 `deploy.sh` 腳本

### 生產環境必要的環境變數
當新增需要設定的功能時：
1. **必須加入 `.env.defaults`** - 包含預設值和註解
2. **必須更新 `docker-compose.prod.yml`** - 如需要則傳遞變數至容器
3. **必須在 `DEPLOYMENT.md` 文件化** - 加入環境變數表格

### 關鍵生產環境變數
```bash
# 這些必須在 EC2/生產伺服器上設定：
SERVER_IP=15.168.229.18              # 您的實際伺服器 IP
VITE_API_URL=http://15.168.229.18:5246/api  # 前端 API URL（建置時）
VITE_MAIN_APP_URL=http://15.168.229.18:5173  # 管理後台用的主應用程式 URL
ALLOWED_ORIGINS=http://15.168.229.18:5173,http://15.168.229.18:5174
```

### 部署檢查清單
推送將要部署的程式碼前：
- [ ] 所有環境變數都存在於 `.env.defaults`
- [ ] 前端建置參數在 `docker-compose.prod.yml` 中（如需要）
- [ ] 生產程式碼中沒有寫死的 localhost URL
- [ ] 考慮生產建置相容性

### 常見部署失敗原因
**請記住：部署失敗通常是因為：**
1. `.env.defaults` 中缺少環境變數
2. 程式碼中寫死了開發環境 URL
3. Dockerfile 中未傳遞建置時變數
4. 需要時未使用 `--build` 標誌

## ⚠️ 重要：Docker 開發環境

**開發者通常已經有 Docker Compose 在執行中！執行任何操作前請先檢查：**

```bash
# 檢查 Docker 容器狀態
docker-compose ps

# 檢查特定埠號是否使用中
lsof -i :5173  # 前端
lsof -i :5246  # API
lsof -i :5174  # 管理後台
```

### 執行中的服務端點
當 Docker Compose 執行時，這些服務可用：
- **前端**：http://localhost:5173
- **API**：http://localhost:5246（Swagger UI：/swagger）
- **管理後台**：http://localhost:5174  
- **MariaDB**：localhost:3306
- **Adminer**（選用）：http://localhost:8080（需要 `--profile tools`）

### ❌ 請勿執行
- **請勿**再次執行 `docker-compose up` - 會造成埠號衝突
- **請勿**在容器內執行 `dotnet run` 或 `npm run dev`
- **請勿**嘗試重新建立現有容器

### ✅ 正確操作
```bash
# 重新啟動服務
docker-compose restart [service-name]

# 查看日誌
docker-compose logs -f [service-name]

# 進入執行中的容器
docker-compose exec api bash
docker-compose exec web sh
docker-compose exec admin sh

# 如果真的需要重新啟動所有服務
docker-compose down && docker-compose up
```

### 開發工作流程
1. **假設 Docker Compose 已在執行中**
2. 在本地編輯程式碼（熱重載會自動套用更改）
3. 對於 .NET 指令，使用 `docker-compose exec api dotnet [command]`
4. 對於 npm 指令，使用 `docker-compose exec web npm [command]` 或 `docker-compose exec admin npm [command]`

## 快速開始指令

```bash
# 首次設定
cp .env.defaults .env
docker-compose up

# 一般開發
docker-compose up

# 本地開發（不使用 Docker）
# 終端機 1：後端
cd BadmintonForum.API && dotnet run

# 終端機 2：主前端  
cd badminton-forum-vue && npm run dev

# 終端機 3：管理後台
cd badminton-forum-admin && npm run dev
```

## 環境設定

### 環境檔案
- **`.env`** - 主要設定（從 `.env.defaults` 複製）
- **`badminton-forum-vue/.env.development`** - 主前端設定（包含預設值）
- **`badminton-forum-admin/.env.development`** - 管理後台設定

### 關鍵環境變數
- `GOOGLE_CLIENT_ID` - 設定此項以啟用 Google OAuth（選用）
- `JWT_SECRET` - 生產環境必須更改
- `MARIADB_PASSWORD` - 生產環境必須更改
- `ADMIN_PORT` - 管理後台埠號（預設：5174）
- `ADMIN_APP_NAME` - 管理後台標題（預設：羽球論壇管理後台）

## 開發憑證

### 資料庫存取
```
MariaDB 連線：
- 主機：localhost（或 Docker 中的 'db'）
- 埠號：3306
- 資料庫：badmintonforumdb
- 使用者名稱：badmintonuser
- 密碼：BadmintonPass123
- Root 密碼：rootpass123

透過 Docker 快速連線：
docker-compose exec db mysql -u badmintonuser -pBadmintonPass123 badmintonforumdb

Adminer 網頁介面：
- URL：http://localhost:8080
- 伺服器：db
- 使用者名稱：badmintonuser
- 密碼：BadmintonPass123
- 資料庫：badmintonforumdb
```

### 測試帳號
```
管理員帳號：
- 電子郵件：123@gmail.com
- 密碼：123456
- 角色：Administrator
- 用途：測試管理後台 http://localhost:5174

一般測試使用者可透過註冊建立
```

## 必要開發指令

### 後端（.NET）
```bash
# 資料庫遷移
dotnet ef migrations add [Name]      # 建立遷移
dotnet ef database update            # 套用遷移

# 測試
dotnet test                          # 執行測試
dotnet format                        # 格式化程式碼（CI 要求）

# 使用者密鑰（開發環境）
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=badmintonforumdb;User=badmintonuser;Password=your-password"
dotnet user-secrets set "JwtSettings:Secret" "your-secret-key-at-least-32-chars"
```

### 前端（Vue）
```bash
# 開發
npm run dev                          # 啟動開發伺服器
npm run build                        # 建置生產版本
npm run test:e2e                     # 執行 E2E 測試
```

### Docker 操作
```bash
docker-compose logs -f [service]     # 查看日誌
docker-compose exec api dotnet ef database update  # 在 Docker 中執行遷移
docker-compose down -v               # 清理所有內容
```

## 高階架構

### 後端結構
```
BadmintonForum.API/
├── Controllers/          # API 端點（Auth、Posts、Admin、Replies、Profile 等）
├── Models/              # 實體模型（User、Post、Reply、Category、PostLike）
├── DTOs/                # 資料傳輸物件
├── Services/            # 商業邏輯（JwtService、EmailService）
├── Data/                # DbContext 和設定
├── Migrations/          # EF Core 遷移
└── migrations-sql/      # 可重複執行的 SQL 腳本，確保安全執行
```

**關鍵模式**：
- 使用 Entity Framework Core 的儲存庫模式
- JWT Bearer 身份驗證搭配自訂 JwtService
- 商業邏輯的服務層
- API 回應的 DTOs
- 全面使用 Async/await

### 主前端結構
```
badminton-forum-vue/
├── src/
│   ├── views/           # 頁面元件（Home、Post、Profile、Settings 等）
│   ├── components/      # 可重用 UI 元件
│   │   ├── ui/          # UI 元件庫
│   │   ├── BadmintonCourtDiagram.vue  # 戰術板編輯器
│   │   ├── BadmintonCourtViewer.vue   # 戰術板檢視器
│   │   ├── ReplyThread.vue            # 回覆討論串元件
│   │   └── RichTextEditor.vue         # TipTap 富文本編輯器
│   ├── api/            # Axios API 客戶端模組
│   ├── stores/         # Pinia 狀態管理
│   └── router/         # Vue Router 設定
└── e2e/                # Playwright E2E 測試（目前停用）
```

### 管理後台結構
```
badminton-forum-admin/
├── src/
│   ├── pages/           # 管理頁面
│   │   ├── admin/dashboard/    # 儀表板與統計
│   │   ├── users/              # 使用者管理
│   │   ├── posts/              # 貼文管理
│   │   ├── categories/         # 分類管理
│   │   ├── replies/            # 回覆管理
│   │   └── auth/               # 管理員登入
│   ├── components/      # Vuestic UI 元件
│   │   ├── navbar/      # 導覽列
│   │   ├── sidebar/     # 側邊導覽
│   │   └── custom/      # 從主應用程式共用的元件
│   ├── api/            # API 客戶端模組
│   ├── stores/         # Pinia 儲存（auth、user）
│   ├── router/         # Vue Router 搭配身份驗證守衛
│   └── layouts/        # App 和 Auth 版面配置
└── public/             # 靜態資源
```

**關鍵模式**：
- Vue 3 + TypeScript 的 Composition API
- Vuestic UI 元件框架
- Pinia 狀態管理
- Axios 攔截器處理 JWT 權杖
- TipTap 富文本編輯
- 管理員身份驗證的路由守衛
- **羽球場地圖**：使用 Konva.js 的互動式戰術板
- **軟刪除**：貼文和回覆支援軟刪除（IsDeleted、DeletedAt）

## 關鍵設定

### 環境設定
- 後端在開發中使用 .NET User Secrets 儲存敏感資料
- 前端使用 `.env` 檔案設定 API URL
- Docker 在 docker-compose 檔案中使用環境變數

### 身份驗證流程
1. 使用者登入 → API 回傳 JWT 權杖
2. 前端將權杖儲存在 localStorage
3. Axios 攔截器將權杖加入所有請求
4. API 在受保護的端點驗證權杖

### 電子郵件服務
- 開發環境：`ConsoleEmailService`（記錄到控制台）
- 生產環境：`EmailService` 搭配 SMTP（MailKit）
- 在 `appsettings.json` 或環境變數中設定

## 測試方法

- **單元測試**：目前最少，會使用 xUnit for .NET
- **E2E 測試**：Playwright 測試已停用（package.json 顯示「E2E 測試已停用」）
- **API 測試**：使用 `/swagger` 的 Swagger UI 或 `BadmintonForum.API.http` 檔案

## CI/CD 管線

GitHub Actions 工作流程：
1. **CI**（`ci-cd.yml`）：在所有分支執行
   - .NET 格式檢查
   - 使用 PostgreSQL 服務的 API 測試（注意：生產環境使用 MariaDB）
   - 前端建置
   - Docker 建置驗證

2. **Security**（`security.yml`）：每週 CodeQL 掃描

3. **Test**（`test-cicd.yml`）：測試 CI/CD 的簡單工作流程

## 開發 URL

- 主前端：http://localhost:5173（公開論壇）
- 管理後台：http://localhost:5174（管理儀表板）
- API：http://localhost:5246
- Swagger：http://localhost:5246/swagger
- Adminer（Docker）：http://localhost:8080（需要 `--profile tools`）

## 工具使用注意事項

### Bash 工具限制
**重要：Bash 工具有您必須了解的特定限制：**

1. **萬用字元（`*`）無法正常運作**
   ```bash
   # ❌ 錯誤 - 會失敗
   ls ~/.ssh/*.pem
   
   # ✅ 正確 - 改用 find
   find ~/.ssh -name "*.pem"
   
   # ✅ 正確 - 明確列出檔案
   ls ~/.ssh/badminton-forum-osaka-key.pem
   ```

2. **沒有輸出的管道指令**
   - 如果管道指令沒有產生輸出，Bash 工具不會顯示任何內容
   - 偵錯時可能會造成困惑

3. **解決方法**
   - 使用 `find` 指令取代萬用字元模式
   - 使用 `2>&1` 查看錯誤訊息
   - 先測試不含管道的指令

## 常見任務

### 新增 API 端點
1. 在 `Controllers/` 建立控制器
2. 如需要在 `DTOs/` 新增 DTOs
3. 在 `Services/` 新增服務邏輯
4. 更新 Swagger 註解
5. 在 `badminton-forum-vue/src/api/` 新增前端 API 客戶端
6. 如果與管理相關，也要新增到 `badminton-forum-admin/src/api/`

### 修改資料庫架構
1. 在 `Models/` 更新模型
2. 執行 `dotnet ef migrations add [Name]`
3. 產生可重複執行的 SQL：`dotnet ef migrations script --idempotent -o migrations-sql/test.sql`
4. 在本地測試 SQL 腳本
5. 如果成功，為所有遷移重新產生：`dotnet ef migrations script --idempotent -o migrations-sql/all-existing.sql`
6. Docker 會在啟動時使用可重複執行的 SQL 自動套用遷移

### ⚠️ 資料庫遷移規則（關鍵 - 必須遵循）

**絕對不要這樣做：**
1. ❌ **絕對不要直接在 Docker 容器中修改資料庫**
2. ❌ **絕對不要使用原始 SQL ALTER TABLE 陳述式**
3. ❌ **絕對不要跳過遷移工作流程**

**永遠要這樣做：**
1. 修改 Model 類別（Models/*.cs）
2. 執行 `dotnet ef migrations add [描述性名稱]`
3. 產生可重複執行的 SQL：`dotnet ef migrations script --idempotent -o migrations-sql/test.sql`
4. 在本地測試 SQL 執行
5. 如果成功，更新：`dotnet ef migrations script --idempotent -o migrations-sql/all-existing.sql`

**為什麼這很重要：**
- MariaDB DDL 操作是非交易性的（無法復原）
- 手動更改會破壞遷移歷史一致性
- 其他開發者無法重現您的更改
- 生產部署將會失敗

**請記住：如果您需要新增欄位，請停止並使用遷移工作流程！**

### 新增前端頁面
**主前端：**
1. 在 `badminton-forum-vue/src/views/` 建立元件
2. 在 `badminton-forum-vue/src/router/index.js` 新增路由
3. 在 `badminton-forum-vue/src/api/` 新增 API 呼叫
4. 如需要更新導覽

**管理後台：**
1. 在 `badminton-forum-admin/src/pages/` 建立元件
2. 在 `badminton-forum-admin/src/router/index.ts` 新增路由
3. 在 `badminton-forum-admin/src/api/` 新增 API 呼叫
4. 在 `NavigationRoutes.ts` 更新側邊欄導覽

## 重要注意事項

- ⚠️ **資料庫更改必須使用 EF Core 遷移 - 絕對不要直接修改資料庫**
- 資料庫預設使用 UTC 時間戳記
- 前端以繁體中文（zh-TW）顯示
- 分類是預定義的：綜合討論、技術交流、裝備討論、賽事專區、地方球友會
- 管理後台是獨立的應用程式，位於 http://localhost:5174（不是路由）
- 主應用程式為管理員使用者顯示管理後台的外部連結
- 個人檔案 URL 使用數字使用者 ID（例如 `/profile/123`）
- 密碼重設權杖 24 小時後過期
- 貼文和回覆支援軟刪除（IsDeleted 標記）
- 戰術板圖表可以嵌入貼文和回覆中
- 富文本編輯器支援格式化、圖片和嵌入圖表

## 管理後台功能

Vuestic Admin 後台提供全面的管理功能：

### 儀表板
- 即時統計（使用者、貼文、回覆、瀏覽數）
- 互動式圖表和資料視覺化
- 每日活動趨勢
- 分類分布分析

### 使用者管理
- 查看所有使用者，支援分頁和搜尋
- 切換使用者啟用/停用狀態
- 授予/撤銷管理員權限
- 按提供者（本地、Google）篩選
- 查看使用者個人檔案（連結到主應用程式）

### 貼文管理
- 查看所有貼文，支援搜尋和篩選
- 置頂/取消置頂貼文
- 鎖定/解鎖貼文回覆
- 軟刪除貼文
- 按分類和作者篩選

### 分類管理
- 建立、編輯、刪除分類
- 設定顯示順序
- 管理分類圖示
- 查看每個分類的貼文數量
- 受保護的刪除（防止刪除有貼文的分類）

### 回覆管理
- 進階搜尋（內容、作者、日期範圍）
- 查看巢狀回覆討論串
- 軟刪除個別回覆
- 批次刪除多個回覆
- 追蹤父子回覆關係

### 身份驗證
- 獨立的管理員登入系統
- JWT 權杖身份驗證
- 管理員角色驗證
- 自動會話管理


## Git 提交指引

### 🔐 重要：隱私資訊安全檢查

**⚠️ 警告：在執行任何 Git 操作前，必須先檢查是否包含隱私資訊！**

#### 提交前必須檢查的隱私資訊
- **API 金鑰和密鑰**：JWT_SECRET、API keys、第三方服務金鑰
- **資料庫憑證**：資料庫密碼、連線字串
- **OAuth 憑證**：Google Client Secret、其他 OAuth 密鑰
- **個人資訊**：真實 email、電話號碼、個人地址
- **生產環境資訊**：實際 IP 位址、生產環境網址、伺服器憑證
- **雲端服務憑證**：AWS credentials、Azure keys、其他雲端服務金鑰
- **.env 檔案**：確保 .env 檔案不會被提交

#### 檢查指令
```bash
# 1. 檢查即將提交的更改（暫存區）
git diff --cached

# 2. 檢查所有未提交的更改
git diff

# 3. 確認 .gitignore 包含敏感檔案
cat .gitignore | grep -E "\.env|secret|key|credential"

# 4. 搜尋可能的敏感資訊
git diff --cached | grep -i -E "password|secret|key|token|credential"

# 5. 列出所有將被提交的檔案
git status
```

#### 如果發現隱私資訊
1. **立即停止提交操作**
2. 使用 `git reset HEAD <file>` 將檔案從暫存區移除
3. 將敏感資訊移至適當位置：
   - 開發環境：使用 `.env` 檔案或 `dotnet user-secrets`
   - 生產環境：使用環境變數或安全的密鑰管理服務
4. 確保 `.gitignore` 包含所有敏感檔案
5. 重新檢查後再提交

#### 最佳實踐
- **永遠不要**將真實的密碼、金鑰寫在程式碼中
- **永遠不要**提交 `.env` 檔案（使用 `.env.defaults` 作為範本）
- **定期檢查** Git 歷史記錄是否包含敏感資訊
- **使用** `git-secrets` 或類似工具自動防止敏感資訊提交

---

### 提交訊息規範

為了維持清晰一致的版本歷史，請遵循以下提交訊息指引：

1.  **提交頻率**：**每完成一個邏輯單元的更改後提交。**養成在完成每個段落或工作區段後提交的習慣。這會建立更細緻的歷史記錄，並使追蹤更改或需要時復原更容易。

2.  **參考現有風格**：提交前，請使用 `git log` 檢查最近的歷史以維持一致的風格。

3.  **語言一致性**：**提交訊息必須使用繁體中文撰寫。**避免在單一提交訊息中混用英文和中文。

4.  **建議格式（Conventional Commits）**：建議遵循 Conventional Commits 格式以獲得結構化且可追蹤的訊息。
    ```
    <type>(<scope>): <subject>
    ```
    - **type**：建議保留英文關鍵字，如 `feat`、`fix`、`docs`、`style`、`refactor`、`test`、`chore` 等，這有助於 CI/CD 工具整合。
    - **scope**：受更改影響的模組。應使用繁體中文，例如 `驗證`、`貼文`、`使用者`。
    - **subject**：繁體中文的簡潔更改描述。
    - **範例**：
      - `feat(驗證): 新增使用者註冊端點`
      - `fix(貼文): 修正分頁邏輯錯誤`
      - `docs(說明文件): 更新安裝說明`
      - `refactor(使用者): 重構個人資料儲存邏輯`