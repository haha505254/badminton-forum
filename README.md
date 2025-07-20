# Badminton Forum 羽球論壇

一個使用 ASP.NET Core Web API + Vue.js 建立的羽球討論論壇。

## 功能特色

- 🔐 會員註冊/登入系統（含忘記密碼功能）
- 📝 發表文章與回覆
- 🏷️ 分類瀏覽（技術討論、裝備評測、比賽資訊、找球友）
- 👤 個人資料管理
- 🛡️ 管理員後台
- 🔍 搜尋功能

## 技術架構

- **後端**: ASP.NET Core 8.0 Web API
- **前端**: Vue 3 + Vite
- **資料庫**: PostgreSQL 16+
- **認證**: JWT Token
- **ORM**: Entity Framework Core
- **狀態管理**: Pinia

## 🚀 完整環境設定指南（從零開始）

> 💡 **適用情境**：電腦重灌、換新電腦、團隊新成員加入

### 前置需求

1. **安裝 .NET 8.0 SDK**
   ```bash
   # Ubuntu/Debian
   wget https://dot.net/v1/dotnet-install.sh
   chmod +x dotnet-install.sh
   ./dotnet-install.sh --version 8.0.0
   
   # 或參考官網：https://dotnet.microsoft.com/download/dotnet/8.0
   ```

2. **安裝 Node.js 18+**
   ```bash
   # 使用 NodeSource 倉庫
   curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
   sudo apt-get install -y nodejs
   ```

3. **安裝 PostgreSQL 16**
   ```bash
   # Ubuntu/Debian
   sudo apt update
   sudo apt install postgresql postgresql-contrib
   
   # 啟動服務
   sudo systemctl start postgresql
   sudo systemctl enable postgresql
   ```

4. **安裝 Git**
   ```bash
   sudo apt install git
   ```

### 步驟 1: Clone 專案

```bash
git clone [你的儲存庫網址]
cd badminton-forum
```

### 步驟 2: 設定 PostgreSQL 資料庫

```bash
# 切換到 postgres 使用者
sudo -u postgres psql

# 在 psql 中執行以下指令
CREATE DATABASE badmintonforumdb;
CREATE USER badmintonuser WITH ENCRYPTED PASSWORD '設定你的密碼';
GRANT ALL PRIVILEGES ON DATABASE badmintonforumdb TO badmintonuser;

-- 給予 schema 權限（重要！）
\c badmintonforumdb
GRANT ALL ON SCHEMA public TO badmintonuser;
\q
```

> ⚠️ **記住你設定的密碼**，後面會用到！

### 步驟 3: 設定後端 API

#### 3.1 安裝 Entity Framework Core 工具

```bash
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
```

#### 3.2 進入後端目錄

```bash
cd BadmintonForum.API
```

#### 3.3 設定敏感資訊（User Secrets）

```bash
# 初始化 User Secrets
dotnet user-REMOVEDs init

# 設定資料庫連接字串（記得替換成你的密碼）
dotnet user-REMOVEDs set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=badmintonforumdb;Username=badmintonuser;Password=你設定的密碼"

# 生成並設定 JWT 密鑰
dotnet user-REMOVEDs set "JwtSettings:Secret" "$(openssl rand -base64 64 | tr -d '\n')"

# 確認設定成功
dotnet user-REMOVEDs list
```

#### 3.4 還原套件並執行資料庫遷移

```bash
# 還原 NuGet 套件
dotnet restore

# 執行資料庫遷移
dotnet ef database update

# 如果遇到權限問題，可能需要：
# sudo -u postgres psql -d badmintonforumdb -c "GRANT CREATE ON SCHEMA public TO badmintonuser;"
```

#### 3.5 啟動後端 API

```bash
dotnet run
```

✅ 後端 API 應該在 http://localhost:5246 執行
✅ Swagger UI 在 http://localhost:5246/swagger

### 步驟 4: 設定前端

開啟**新的終端機視窗**：

```bash
# 從專案根目錄進入前端資料夾
cd badminton-forum-vue

# 安裝相依套件
npm install

# 複製環境變數檔案
cp .env.example .env.development

# 啟動開發伺服器
npm run dev
```

✅ 前端應該在 http://localhost:5173 執行

## 🧪 測試專案是否正常運作

1. 開啟瀏覽器，前往 http://localhost:5173
2. 嘗試註冊新帳號
3. 登入系統
4. 發表文章測試

## 📝 重要檔案說明

### 敏感資訊位置

- **User Secrets**: `~/.microsoft/userREMOVEDs/[專案ID]/REMOVEDs.json`
- **不會**被 Git 追蹤，很安全

### 查看目前的設定

```bash
# 在 BadmintonForum.API 目錄下
dotnet user-REMOVEDs list
```

## 🔧 常用維護指令

### 資料庫相關

```bash
# 建立新的遷移（當修改 Model 後）
dotnet ef migrations add [遷移名稱]

# 更新資料庫
dotnet ef database update

# 回復到特定遷移
dotnet ef database update [遷移名稱]

# 移除最後一個遷移
dotnet ef migrations remove
```

### 更改資料庫密碼

1. 在 PostgreSQL 中更改：
   ```bash
   sudo -u postgres psql
   ALTER USER badmintonuser WITH PASSWORD '新密碼';
   \q
   ```

2. 更新 User Secrets：
   ```bash
   cd BadmintonForum.API
   dotnet user-REMOVEDs set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=badmintonforumdb;Username=badmintonuser;Password=新密碼"
   ```

### 重新生成 JWT 密鑰

```bash
dotnet user-REMOVEDs set "JwtSettings:Secret" "$(openssl rand -base64 64 | tr -d '\n')"
```

## 🐛 疑難排解

### 問題：資料庫連線失敗

1. 確認 PostgreSQL 正在執行：
   ```bash
   sudo systemctl status postgresql
   ```

2. 確認可以用 psql 連線：
   ```bash
   psql -U badmintonuser -d badmintonforumdb -h localhost
   ```

3. 檢查 User Secrets 設定：
   ```bash
   dotnet user-REMOVEDs list
   ```

### 問題：前端無法連接後端

1. 確認後端正在 http://localhost:5246 執行
2. 檢查 `.env.development` 的 API URL 設定
3. 查看瀏覽器 Console 錯誤訊息

### 問題：Entity Framework 指令找不到

```bash
# 重新安裝並設定 PATH
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
```

## 🚀 部署到生產環境

### 使用環境變數（推薦）

```bash
# Linux/macOS
export ConnectionStrings__DefaultConnection="Host=prod-server;Database=proddb;Username=produser;Password=prodpass"
export JwtSettings__Secret="生產環境的密鑰"

# Windows PowerShell
$env:ConnectionStrings__DefaultConnection="Host=prod-server;..."
$env:JwtSettings__Secret="生產環境的密鑰"
```

### 前端建置

```bash
cd badminton-forum-vue
npm run build
# dist 資料夾即為部署檔案
```

## 🔐 安全注意事項

1. **永遠不要**將密碼提交到 Git
2. **永遠不要**在 appsettings.json 中放真實密碼
3. 定期更換密碼和密鑰
4. 生產環境使用強密碼（16+ 字元）

## 📂 專案結構

```
badminton-forum/
├── BadmintonForum.API/        # ASP.NET Core Web API
│   ├── Controllers/           # API 控制器
│   ├── Models/               # 資料模型
│   ├── DTOs/                 # 資料傳輸物件
│   ├── Services/             # 商業邏輯
│   ├── Migrations/           # EF Core 遷移
│   └── appsettings.json      # 設定檔（不含密碼）
├── badminton-forum-vue/       # Vue.js 前端
│   ├── src/
│   │   ├── views/           # 頁面元件
│   │   ├── components/      # 共用元件
│   │   ├── api/            # API 呼叫
│   │   ├── stores/         # Pinia 狀態管理
│   │   └── router/         # 路由設定
│   └── .env.example        # 環境變數範例
├── .gitignore              # Git 忽略檔案
├── SECURITY.md             # 安全設定指南
└── README.md               # 本文件
```

## 📞 需要幫助？

如果遇到問題：
1. 檢查本文件的疑難排解章節
2. 查看 `SECURITY.md` 了解安全設定
3. 檢查專案的 Issues
4. 開新的 Issue 詢問

---

💡 **記得**：這份文件包含所有從零開始設定專案的步驟，請妥善保存！