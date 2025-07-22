# PostgreSQL 手動安裝和設置指南

## 1. 安裝 PostgreSQL

在終端中執行以下命令：

```bash
# 更新套件列表
sudo apt update

# 安裝 PostgreSQL
sudo apt install -y postgresql postgresql-contrib
```

## 2. 啟動 PostgreSQL 服務

```bash
# 啟動服務
sudo systemctl start postgresql

# 設置開機自動啟動
sudo systemctl enable postgresql

# 檢查服務狀態
sudo systemctl status postgresql
```

## 3. 創建資料庫和用戶

### 方法 A: 使用 psql 命令行

```bash
# 切換到 postgres 用戶
sudo -u postgres psql

# 在 psql 提示符下執行以下 SQL 命令：
CREATE USER badmintonuser WITH PASSWORD 'BadmintonPass123';
CREATE DATABASE BadmintonForumDb OWNER badmintonuser;
GRANT ALL PRIVILEGES ON DATABASE BadmintonForumDb TO badmintonuser;
\q
```

### 方法 B: 使用單行命令

```bash
# 創建用戶
sudo -u postgres createuser -P badmintonuser
# (系統會提示輸入密碼，輸入: BadmintonPass123)

# 創建資料庫
sudo -u postgres createdb -O badmintonuser BadmintonForumDb
```

## 4. 配置 PostgreSQL 連接（如果需要）

如果遇到連接問題，可能需要編輯 PostgreSQL 配置：

```bash
# 找到配置文件位置
sudo -u postgres psql -c "SHOW config_file;"

# 通常在 /etc/postgresql/[版本]/main/postgresql.conf
# 編輯 pg_hba.conf 文件
sudo nano /etc/postgresql/*/main/pg_hba.conf
```

確保有以下行（允許本地連接）：
```
# TYPE  DATABASE        USER            ADDRESS                 METHOD
local   all             all                                     md5
host    all             all             127.0.0.1/32            md5
```

修改後重啟服務：
```bash
sudo systemctl restart postgresql
```

## 5. 測試連接

```bash
# 測試連接到新建的資料庫
psql -h localhost -U badmintonuser -d BadmintonForumDb
# 輸入密碼: BadmintonPass123
```

## 6. 更新專案配置

更新 `BadmintonForum.API/appsettings.json` 中的連接字串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=BadmintonForumDb;Username=badmintonuser;Password=BadmintonPass123"
  }
}
```

## 7. 執行 Entity Framework 遷移

在專案目錄中執行：

```bash
cd BadmintonForum.API

# 安裝 EF Core 工具（如果還沒安裝）
dotnet tool install --global dotnet-ef

# 創建初始遷移
dotnet ef migrations add InitialCreate

# 應用遷移到資料庫
dotnet ef database update
```

## 故障排除

### 如果無法連接到資料庫：

1. 檢查 PostgreSQL 服務是否運行：
   ```bash
   sudo systemctl status postgresql
   ```

2. 檢查是否可以用 postgres 用戶連接：
   ```bash
   sudo -u postgres psql
   ```

3. 檢查防火牆設置（如果從遠程連接）

### 如果忘記密碼：

```bash
# 以 postgres 用戶登入
sudo -u postgres psql

# 修改密碼
ALTER USER badmintonuser WITH PASSWORD '新密碼';
```