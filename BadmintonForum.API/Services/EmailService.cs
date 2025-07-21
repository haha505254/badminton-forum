using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BadmintonForum.API.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string username, string resetToken);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string username, string resetToken)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["Email:FromName"] ?? "羽球論壇",
                    _configuration["Email:FromAddress"] ?? "noreply@badminton-forum.com"
                ));
                message.To.Add(new MailboxAddress(username, toEmail));
                message.Subject = "重置您的羽球論壇密碼";

                var baseUrl = _configuration["Email:BaseUrl"] ?? "http://localhost:5173";
                var resetUrl = $"{baseUrl}/reset-password?token={resetToken}";

                var builder = new BodyBuilder
                {
                    HtmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f5f5f5;'>
                            <div style='background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                                <h2 style='color: #2c3e50; text-align: center;'>重置密碼請求</h2>
                                <p>親愛的 {username}，</p>
                                <p>我們收到了您重置密碼的請求。請點擊下方按鈕來設定新密碼：</p>
                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='{resetUrl}' style='background-color: #3498db; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>重置密碼</a>
                                </div>
                                <p>或者複製以下連結到瀏覽器：</p>
                                <p style='background-color: #f8f9fa; padding: 10px; border-radius: 5px; word-break: break-all;'>{resetUrl}</p>
                                <p style='color: #e74c3c;'><strong>注意：</strong>此連結將在 24 小時後失效。</p>
                                <p>如果您沒有請求重置密碼，請忽略此郵件。</p>
                                <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                                <p style='color: #7f8c8d; font-size: 12px; text-align: center;'>
                                    此郵件由羽球論壇系統自動發送，請勿回覆。<br>
                                    如有問題，請聯繫我們的客服團隊。
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>",
                    TextBody = $@"
親愛的 {username}，

我們收到了您重置密碼的請求。請訪問以下連結來設定新密碼：

{resetUrl}

注意：此連結將在 24 小時後失效。

如果您沒有請求重置密碼，請忽略此郵件。

此郵件由羽球論壇系統自動發送，請勿回覆。"
                };

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();

                // 連接到 SMTP 伺服器
                await client.ConnectAsync(
                    _configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
                    int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                    SecureSocketOptions.StartTls
                );

                // 如果配置了使用者名稱和密碼，則進行驗證
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
                {
                    await client.AuthenticateAsync(smtpUsername, smtpPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Password reset email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {toEmail}");
                throw;
            }
        }
    }
}