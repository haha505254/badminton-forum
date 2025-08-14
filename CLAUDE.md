# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Full-stack badminton forum application:
- **Backend**: ASP.NET Core 8.0 Web API with MariaDB
- **Frontend**: Vue 3 SPA with Vite
- **Architecture**: RESTful API with JWT authentication + Google OAuth 2.0

## Quick Start Commands

```bash
# First time setup (超簡單！)
cp .env.defaults .env
docker-compose up

# Regular development
docker-compose up

# Local development (without Docker)
# Terminal 1: Backend
cd BadmintonForum.API && dotnet run

# Terminal 2: Frontend  
cd badminton-forum-vue && npm run dev
```

## Environment Setup

### Environment Files
- **`.env`** - Main configuration (copy from `.env.defaults`)
- **`badminton-forum-vue/.env.development`** - Frontend config (已包含預設值)

### Key Environment Variables
- `GOOGLE_CLIENT_ID` - 需自行設定以啟用 Google OAuth (optional)
- `JWT_SECRET` - 生產環境必須更改
- `MARIADB_PASSWORD` - 生產環境必須更改

## Essential Development Commands

### Backend (.NET)
```bash
# Database migrations
dotnet ef migrations add [Name]      # Create migration
dotnet ef database update            # Apply migrations

# Testing
dotnet test                          # Run tests
dotnet format                        # Format code (required by CI)

# User secrets (development)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=badmintonforumdb;User=badmintonuser;Password=your-password"
dotnet user-secrets set "JwtSettings:Secret" "your-secret-key-at-least-32-chars"
```

### Frontend (Vue)
```bash
# Development
npm run dev                          # Start dev server
npm run build                        # Build for production
npm run test:e2e                     # Run E2E tests
```

### Docker Operations
```bash
docker-compose logs -f [service]     # View logs
docker-compose exec api dotnet ef database update  # Run migrations in Docker
docker-compose down -v               # Clean everything
```

## High-Level Architecture

### Backend Structure
```
BadmintonForum.API/
├── Controllers/          # API endpoints (Auth, Posts, Admin, etc.)
├── Models/              # Entity models (User, Post, Reply, Category)
├── DTOs/                # Data transfer objects
├── Services/            # Business logic (JwtService, EmailService)
├── Data/                # DbContext and configurations
└── Migrations/          # EF Core migrations
```

**Key patterns**:
- Repository pattern with Entity Framework Core
- JWT Bearer authentication with custom JwtService
- Service layer for business logic
- DTOs for API responses
- Async/await throughout

### Frontend Structure
```
badminton-forum-vue/
├── src/
│   ├── views/           # Page components
│   ├── components/      # Reusable UI components
│   ├── api/            # Axios API client modules
│   ├── stores/         # Pinia state management
│   └── router/         # Vue Router configuration
└── e2e/                # Playwright E2E tests
```

**Key patterns**:
- Composition API with Vue 3
- Pinia for state management (auth, posts stores)
- Axios interceptors for JWT token handling
- TipTap for rich text editing
- Route guards for authentication

## Critical Configuration

### Environment Setup
- Backend uses .NET User Secrets for sensitive data in development
- Frontend uses `.env` files for API URL configuration
- Docker uses environment variables in docker-compose files

### Authentication Flow
1. User login → API returns JWT token
2. Frontend stores token in localStorage
3. Axios interceptor adds token to all requests
4. API validates token on protected endpoints

### Email Service
- Development: `ConsoleEmailService` (logs to console)
- Production: `EmailService` with SMTP (MailKit)
- Configure in `appsettings.json` or environment variables

## Testing Approach

- **Unit tests**: Currently minimal, would use xUnit for .NET
- **E2E tests**: Playwright tests in `badminton-forum-vue/e2e/`
- **API testing**: Use Swagger UI at `/swagger` or the `.http` file

## CI/CD Pipeline

GitHub Actions workflows:
1. **CI** (`ci.yml`): Runs on all PRs
   - .NET format check
   - API tests with PostgreSQL service
   - Frontend build and E2E tests
   - Docker build verification

2. **Security** (`security.yml`): Weekly CodeQL scans

3. **Deployment**: Automatic to production on main branch merge

## Development URLs

- Frontend: http://localhost:5173
- API: http://localhost:5246
- Swagger: http://localhost:5246/swagger
- Adminer (Docker): http://localhost:8080

## Common Tasks

### Add a new API endpoint
1. Create controller in `Controllers/`
2. Add DTOs if needed in `DTOs/`
3. Add service logic in `Services/`
4. Update Swagger annotations
5. Add frontend API client in `badminton-forum-vue/src/api/`

### Modify database schema
1. Update model in `Models/`
2. Run `dotnet ef migrations add [Name]`
3. Review generated migration
4. Run `dotnet ef database update`

### ⚠️ 資料庫修改嚴格規則 (CRITICAL - MUST FOLLOW)

**絕對禁止事項：**
1. ❌ **絕對不可以直接在 Docker 容器內手動修改資料庫**
2. ❌ **絕對不可以用 SQL 直接 ALTER TABLE**
3. ❌ **絕對不可以跳過 Migration 流程**

**唯一正確流程：**
1. 修改 Model class (Models/*.cs)
2. 執行 `dotnet ef migrations add [DescriptiveName]`
3. 生成 idempotent SQL: `dotnet ef migrations script --idempotent -o migrations-sql/test.sql`
4. 測試 SQL 執行
5. 確認無誤後更新: `dotnet ef migrations script --idempotent -o migrations-sql/all-existing.sql`

**為什麼這很重要：**
- MariaDB DDL 操作無法回滾
- 手動修改會造成 Migration 歷史不一致
- 其他開發者無法重現你的修改
- 部署到生產環境會失敗

**記住：如果你發現需要加欄位，停下來，走 Migration 流程！**

### Add new frontend page
1. Create component in `views/`
2. Add route in `router/index.js`
3. Add API calls in `api/`
4. Update navigation if needed

## Important Notes

- ⚠️ **Database changes MUST use EF Core Migrations - NEVER modify database directly**
- Database uses UTC timestamps by default
- Frontend displays in Traditional Chinese (zh-TW)
- Categories are predefined: 技術討論, 裝備評測, 比賽資訊, 找球友
- Admin panel accessible at `/admin` for users with admin role
- Profile URLs use numeric user IDs (e.g., `/profile/123`)
- Password reset tokens expire after 24 hours

## Git Commit Guidelines

To maintain a clear and consistent version history, please follow these commit message guidelines:

1.  **Commit Frequency**: **Commit after each logical unit of change.** Make it a habit to commit after completing each paragraph or section of work. This creates a more granular history and makes it easier to track changes or revert if needed.

2.  **Refer to Existing Style**: Before committing, please check the recent history with `git log` to maintain a consistent style.

3.  **Language Consistency**: **Commit messages MUST be written in Traditional Chinese.** Avoid mixing English and Chinese within a single commit message.

4.  **Recommended Format (Conventional Commits)**: It is recommended to follow the Conventional Commits format for structured and trackable messages.
    ```
    <type>(<scope>): <subject>
    ```
    - **type**: It is recommended to keep the English keywords, such as `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, etc., as this helps with CI/CD tool integration.
    - **scope**: The module affected by the change. This should be in Traditional Chinese, e.g., `驗證`, `貼文`, `使用者`.
    - **subject**: A concise description of the change in Traditional Chinese.
    - **Examples**:
      - `feat(驗證): 新增使用者註冊端點`
      - `fix(貼文): 修正分頁邏輯錯誤`
      - `docs(說明文件): 更新安裝說明`
      - `refactor(使用者): 重構個人資料儲存邏輯`