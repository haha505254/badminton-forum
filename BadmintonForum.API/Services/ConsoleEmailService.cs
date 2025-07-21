using Microsoft.Extensions.Logging;

namespace BadmintonForum.API.Services
{
    /// <summary>
    /// 開發用的 Email 服務 - 將重置連結輸出到控制台和日誌
    /// </summary>
    public class ConsoleEmailService : IEmailService
    {
        private readonly ILogger<ConsoleEmailService> _logger;
        private readonly IConfiguration _configuration;

        public ConsoleEmailService(ILogger<ConsoleEmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task SendPasswordResetEmailAsync(string toEmail, string username, string resetToken)
        {
            var baseUrl = _configuration["Email:BaseUrl"] ?? "http://localhost:5173";
            var resetUrl = $"{baseUrl}/reset-password?token={resetToken}";

            // 輸出到日誌
            _logger.LogWarning($"[開發模式] 密碼重置連結已產生 - Email: {toEmail}, 連結: {resetUrl}");

            // 在控制台以明顯方式顯示
            Console.WriteLine("\n");
            Console.WriteLine("┌─────────────────────────────────────────────────────────────┐");
            Console.WriteLine("│                    🔐 密碼重置連結                          │");
            Console.WriteLine("├─────────────────────────────────────────────────────────────┤");
            Console.WriteLine($"│ 收件人: {toEmail,-51} │");
            Console.WriteLine($"│ 使用者: {username,-51} │");
            Console.WriteLine("├─────────────────────────────────────────────────────────────┤");
            Console.WriteLine("│ 請複製下方連結到瀏覽器:                                    │");
            Console.WriteLine("│                                                             │");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"│ {resetUrl}");
            Console.ResetColor();
            Console.WriteLine("│                                                             │");
            Console.WriteLine("│ ⏰ 此連結 24 小時後失效                                    │");
            Console.WriteLine("└─────────────────────────────────────────────────────────────┘");
            Console.WriteLine("\n");

            return Task.CompletedTask;
        }
    }
}