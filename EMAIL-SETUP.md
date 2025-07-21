# Email 設定指南

本指南說明如何設定 Email 服務以支援忘記密碼功能。

## Gmail 設定步驟

### 1. 啟用兩步驟驗證
1. 前往 [Google 帳戶設定](https://myaccount.google.com/security)
2. 啟用「兩步驟驗證」

### 2. 建立應用程式密碼
1. 在安全性設定中，找到「應用程式密碼」
2. 產生一個新的應用程式密碼
3. 記下這個 16 位密碼

### 3. 設定 appsettings.Development.json

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-16-digit-app-password",
    "FromName": "羽球論壇",
    "FromAddress": "your-email@gmail.com",
    "BaseUrl": "http://localhost:5173"
  }
}
```

## 其他 Email 服務商

### SendGrid
```json
{
  "Email": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": "587",
    "SmtpUsername": "apikey",
    "SmtpPassword": "your-sendgrid-api-key",
    "FromName": "羽球論壇",
    "FromAddress": "noreply@yourdomain.com",
    "BaseUrl": "https://yourdomain.com"
  }
}
```

### Mailgun
```json
{
  "Email": {
    "SmtpHost": "smtp.mailgun.org",
    "SmtpPort": "587",
    "SmtpUsername": "postmaster@yourdomain.mailgun.org",
    "SmtpPassword": "your-mailgun-password",
    "FromName": "羽球論壇",
    "FromAddress": "noreply@yourdomain.com",
    "BaseUrl": "https://yourdomain.com"
  }
}
```

## 生產環境設定

在生產環境，請使用環境變數：

```bash
export Email__SmtpHost="smtp.gmail.com"
export Email__SmtpPort="587"
export Email__SmtpUsername="your-email@gmail.com"
export Email__SmtpPassword="your-app-password"
export Email__FromName="羽球論壇"
export Email__FromAddress="noreply@badminton-forum.com"
export Email__BaseUrl="https://badminton-forum.com"
```

## 測試 Email 功能

1. 啟動後端 API：
   ```bash
   cd BadmintonForum.API
   dotnet run
   ```

2. 啟動前端：
   ```bash
   cd badminton-forum-vue
   npm run dev
   ```

3. 測試流程：
   - 前往 http://localhost:5173/login
   - 點擊「忘記密碼？」
   - 輸入註冊的 Email
   - 檢查郵箱收到重置連結
   - 點擊連結設定新密碼

## 故障排除

### 常見問題

1. **無法發送郵件**
   - 確認應用程式密碼正確
   - 檢查防火牆是否允許 SMTP 連線
   - 確認 Gmail 帳戶允許「低安全性應用程式存取」

2. **郵件進入垃圾郵件**
   - 設定 SPF 和 DKIM 記錄
   - 使用專業的 Email 服務商
   - 確保郵件內容不含垃圾郵件關鍵字

3. **連線逾時**
   - 檢查 SMTP 連接埠是否正確
   - 某些網路環境可能封鎖 SMTP 連接埠

## 安全注意事項

1. **絕不要將密碼提交到版本控制**
2. **使用 User Secrets 或環境變數儲存敏感資訊**
3. **定期更換應用程式密碼**
4. **監控異常的郵件發送活動**

## 開發環境快速測試

如果不想設定真實的 Email 服務，可以使用以下替代方案：

### 1. 使用 Mailtrap（推薦）
免費的 Email 測試服務：
```json
{
  "Email": {
    "SmtpHost": "smtp.mailtrap.io",
    "SmtpPort": "2525",
    "SmtpUsername": "your-mailtrap-username",
    "SmtpPassword": "your-mailtrap-password",
    "FromName": "羽球論壇",
    "FromAddress": "test@badminton-forum.com",
    "BaseUrl": "http://localhost:5173"
  }
}
```

### 2. 使用 Console Email（開發用）
修改 `EmailService.cs` 將郵件內容輸出到控制台：
```csharp
// 在 SendPasswordResetEmailAsync 方法中加入：
_logger.LogInformation($"Password Reset Email:\nTo: {toEmail}\nReset URL: {resetUrl}");
```