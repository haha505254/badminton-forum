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
├── Controllers/          # API endpoints (Auth, Posts, Admin, Replies, Profile, etc.)
├── Models/              # Entity models (User, Post, Reply, Category, PostLike)
├── DTOs/                # Data transfer objects
├── Services/            # Business logic (JwtService, EmailService)
├── Data/                # DbContext and configurations
├── Migrations/          # EF Core migrations
└── migrations-sql/      # Idempotent SQL scripts for safe re-execution
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
│   ├── views/           # Page components (Home, Post, Profile, Admin, Settings, etc.)
│   ├── components/      # Reusable UI components
│   │   ├── ui/          # UI component library
│   │   ├── BadmintonCourtDiagram.vue  # Tactical board editor
│   │   ├── BadmintonCourtViewer.vue   # Tactical board viewer
│   │   ├── ReplyThread.vue            # Reply thread component
│   │   └── RichTextEditor.vue         # TipTap rich text editor
│   ├── api/            # Axios API client modules
│   ├── stores/         # Pinia state management
│   └── router/         # Vue Router configuration
└── e2e/                # Playwright E2E tests (currently disabled)
```

**Key patterns**:
- Composition API with Vue 3
- Pinia for state management (auth, posts stores)
- Axios interceptors for JWT token handling
- TipTap for rich text editing
- Route guards for authentication
- **Badminton Court Diagram**: Interactive tactical board using Konva.js
- **Soft Delete**: Posts and Replies support soft deletion (IsDeleted, DeletedAt)

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
- **E2E tests**: Playwright tests disabled (package.json shows "E2E tests are disabled")
- **API testing**: Use Swagger UI at `/swagger` or `BadmintonForum.API.http` file

## CI/CD Pipeline

GitHub Actions workflows:
1. **CI** (`ci-cd.yml`): Runs on all branches
   - .NET format check
   - API tests with PostgreSQL service (Note: production uses MariaDB)
   - Frontend build
   - Docker build verification

2. **Security** (`security.yml`): Weekly CodeQL scans

3. **Test** (`test-cicd.yml`): Simple workflow for testing CI/CD

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
3. Generate idempotent SQL: `dotnet ef migrations script --idempotent -o migrations-sql/test.sql`
4. Test the SQL script locally
5. If successful, regenerate for all migrations: `dotnet ef migrations script --idempotent -o migrations-sql/all-existing.sql`
6. Docker will automatically apply migrations on startup using the idempotent SQL

### ⚠️ Database Migration Rules (CRITICAL - MUST FOLLOW)

**NEVER DO THIS:**
1. ❌ **NEVER modify database directly in Docker containers**
2. ❌ **NEVER use raw SQL ALTER TABLE statements**
3. ❌ **NEVER skip the Migration workflow**

**ALWAYS DO THIS:**
1. Modify Model class (Models/*.cs)
2. Run `dotnet ef migrations add [DescriptiveName]`
3. Generate idempotent SQL: `dotnet ef migrations script --idempotent -o migrations-sql/test.sql`
4. Test the SQL execution locally
5. If successful, update: `dotnet ef migrations script --idempotent -o migrations-sql/all-existing.sql`

**Why this matters:**
- MariaDB DDL operations are non-transactional (cannot rollback)
- Manual changes break Migration history consistency
- Other developers cannot reproduce your changes
- Production deployments will fail

**REMEMBER: If you need to add a column, STOP and use the Migration workflow!**

### Add new frontend page
1. Create component in `views/`
2. Add route in `router/index.js`
3. Add API calls in `api/`
4. Update navigation if needed

## Important Notes

- ⚠️ **Database changes MUST use EF Core Migrations - NEVER modify database directly**
- Database uses UTC timestamps by default
- Frontend displays in Traditional Chinese (zh-TW)
- Categories are predefined: 綜合討論區, 技術交流區, 裝備討論區, 賽事專區, 地區球友會
- Admin panel accessible at `/admin` for users with admin role
- Profile URLs use numeric user IDs (e.g., `/profile/123`)
- Password reset tokens expire after 24 hours
- Posts and Replies support soft deletion (IsDeleted flag)
- Tactical board diagrams can be embedded in posts and replies
- Rich text editor supports formatting, images, and embedded diagrams

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