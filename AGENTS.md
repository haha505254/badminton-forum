# Repository Guidelines

## 專案結構與模組分工
BadmintonForum.API 模組提供 ASP.NET Core 8 API，Controllers、Services、DTOs、Data 與 EF Core Migrations 都集中於此，敏感設定示例請參考 BadmintonForum.API/SECURITY.md。badminton-forum-vue 目錄承載公開論壇，src/ 內含 views、components、stores 與 api 客戶端，public/ 為靜態資源，tailwind.config.js 管理樣式。badminton-forum-admin 則是 Vuestic Admin 後台，TypeScript 原始碼位於 src/，.github 子目錄提供 ISSUE、PR 與 COMMIT 模板。docker 與 docker-compose*.yml 描述本地與生產編排，deploy.sh、EC2_COMMANDS.md 以及 docs/ 下的 CICD_SETUP.md、EMAIL-SETUP.md、GOOGLE_OAUTH_SETUP.md 等文件記錄部署與整合指引；若需手動資料庫流程，可參考 docs/setup-database-manual.md。

## 建置、測試與開發指令
`cp .env.defaults .env` 初始化環境變數後，可用 `docker-compose up -d` 啟動完整堆疊；生產部署請執行 `./deploy.sh` 或 `docker compose -f docker-compose.prod.yml up -d --build`。後端獨立開發時執行 `dotnet restore && dotnet run --project BadmintonForum.API`，需要遷移時以 `dotnet ef migrations add <Name>` 與 `dotnet ef database update` 搭配 migrations-sql 腳本。前端各自於目錄內 `npm install` 後使用 `npm run dev`，管理後台保留埠號可加 `-- --port 5174`。CI 相關流程集中於 .github/workflows/ci-cd.yml，可於需求時手動觸發。

## 程式風格與命名規範
遵循 CLAUDE.md 指定的繁體中文文件與註解政策。C# 採四空白縮排，類別與公開成員使用 PascalCase，本地變數採 camelCase，命名空間需對應資料夾層級，並於 Program.cs 註冊新增服務。Vue 與 TypeScript 依 badminton-forum-admin/.editorconfig 裁切為兩空白縮排；單檔元件建議 kebab-case 命名並於 src/composables 共置組合函式，Pinia store 維持 PascalCase 名稱而以 use 前綴。樣式請善用 Tailwind 工具類別並保持由版面至視覺的排序，必要時執行 `npm run lint` 或 `npm run format`。

## 測試指引
目前自動化尚在起步，新增 API 功能時請建立 BadmintonForum.API.Tests xUnit 專案，優先涵蓋驗證、貼文、審核與郵件流程，同步更新 migrations-sql 腳本。前端 Playwright 依賴已安裝但 scripts 為佔位，未來請將案例置於 e2e/ 並恢復 `npm run test:e2e`。在 PR 說明中記錄手動檢測結果，至少覆蓋登入、貼文、回覆、戰術板與忘記密碼流程，同時確認 `npm run build` 於兩個前端皆可通過且 `dotnet ef database update` 能在乾淨資料庫運行。

## Commit 與 Pull Request 準則
根據 git log 與 badminton-forum-admin/.github/COMMIT_CONVENTION.md，提交訊息採 `type(scope): 摘要`，type 使用 feat、fix、docs、refactor 等英文關鍵字，scope 建議以模組或功能的繁體中文表示，例如 `fix(管理後台): 修正貼文狀態顯示問題`。維持一個邏輯單元一個提交並於內容連結相關議題。建立 PR 前先自檢：同步更新 `.env.defaults` 或 docker-compose 變數、附上前端截圖或錄影、列出執行的指令與測試、說明是否新增遷移與需要的部署步驟，並對齊 CLAUDE.md 的部署檢查清單。

## 安全與設定提示
請參照 BadmintonForum.API/SECURITY.md 透過 User Secrets 或環境變數管理 ConnectionStrings、JwtSettings、Email 認證等敏感資訊，勿把憑證寫入 Git。`cp .env.defaults .env` 後務必調整 JWT_SECRET、MARIADB_PASSWORD、DEFAULT_ADMIN_* 等預設值，並在生產環境同步更新 deploy/docker-compose.prod.yml 或雲端參數。Google OAuth、MailKit SMTP 與 Docker 忘記密碼流程分別記錄於 docs/GOOGLE_OAUTH_SETUP.md、docs/EMAIL-SETUP.md、docs/DOCKER-EMAIL-TEST.md。遠端部署需使用 `scripts/db-tunnel.sh` 或 AWS Systems Manager 參數存放密鑰，保持服務端點與 CORS ALLOWED_ORIGINS 與時俱進，避免硬編碼 localhost。
