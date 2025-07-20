# 安全性設定指南

## 敏感資訊管理

本專案包含以下敏感資訊，**絕對不要**將這些資訊提交到版本控制系統：

1. **資料庫連接字串** - 包含資料庫密碼
2. **JWT Secret Key** - 用於簽署和驗證 JWT 令牌
3. **API 密鑰** - 任何第三方服務的 API 密鑰

## 使用 .NET User Secrets（推薦用於開發環境）

### 1. 初始化 User Secrets

在 `BadmintonForum.API` 目錄下執行：

```bash
dotnet user-REMOVEDs init
```

### 2. 設定敏感資訊

```bash
# 設定資料庫連接字串
dotnet user-REMOVEDs set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=badmintonforumdb;Username=youruser;Password=yourREMOVED"

# 設定 JWT Secret
dotnet user-REMOVEDs set "JwtSettings:Secret" "YourVerySecretKeyAtLeast32CharactersLong1234567890"
```

### 3. 查看已設定的 REMOVEDs

```bash
dotnet user-REMOVEDs list
```

### 4. 移除 REMOVED

```bash
dotnet user-REMOVEDs remove "ConnectionStrings:DefaultConnection"
```

## 使用環境變數（適用於生產環境）

### Linux/macOS

```bash
export ConnectionStrings__DefaultConnection="Host=prod-server;Database=proddb;Username=produser;Password=prodpass"
export JwtSettings__Secret="ProductionSecretKeyAtLeast32CharactersLong"
```

### Windows (Command Prompt)

```cmd
set ConnectionStrings__DefaultConnection=Host=prod-server;Database=proddb;Username=produser;Password=prodpass
set JwtSettings__Secret=ProductionSecretKeyAtLeast32CharactersLong
```

### Windows (PowerShell)

```powershell
$env:ConnectionStrings__DefaultConnection="Host=prod-server;Database=proddb;Username=produser;Password=prodpass"
$env:JwtSettings__Secret="ProductionSecretKeyAtLeast32CharactersLong"
```

## 使用 appsettings.Development.json（不推薦）

如果你選擇使用 `appsettings.Development.json`：

1. 複製 `appsettings.Example.json` 為 `appsettings.Development.json`
2. 填入實際的敏感資訊
3. **確保** `.gitignore` 包含 `appsettings.Development.json`
4. **絕對不要**提交這個檔案到版本控制

## 生成安全的 JWT Secret

使用以下方法生成安全的 JWT Secret：

### C# 程式碼

```csharp
using System.Security.Cryptography;

var key = new byte[64]; // 512 bits
RandomNumberGenerator.Fill(key);
var base64Key = Convert.ToBase64String(key);
Console.WriteLine(base64Key);
```

### Linux/macOS 命令列

```bash
openssl rand -base64 64
```

### PowerShell

```powershell
[Convert]::ToBase64String((1..64 | ForEach {Get-Random -Maximum 256}))
```

## 安全檢查清單

在提交程式碼前，請確認：

- [ ] `appsettings.json` 不包含任何實際的密碼或密鑰
- [ ] 沒有在程式碼中硬編碼任何敏感資訊
- [ ] `.gitignore` 包含所有敏感設定檔
- [ ] 使用了 User Secrets 或環境變數管理敏感資訊
- [ ] JWT Secret 至少有 32 個字元長度
- [ ] 資料庫密碼是強密碼
- [ ] 生產環境使用不同的密碼和密鑰

## 如果不小心提交了敏感資訊

如果你不小心將敏感資訊提交到 Git：

1. **立即更改所有洩露的密碼和密鑰**
2. 使用 `git filter-branch` 或 BFG Repo-Cleaner 從歷史記錄中移除敏感資訊
3. Force push 到遠端儲存庫
4. 通知所有團隊成員重新 clone 儲存庫

記住：一旦敏感資訊被推送到公開儲存庫，就應該視為已經洩露，必須立即更換。