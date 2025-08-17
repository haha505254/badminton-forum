using BadmintonForum.API.Data;
using BadmintonForum.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BadmintonForum.API.Services
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(ApplicationDbContext context, IConfiguration configuration, ILogger<DataSeeder> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // 種子分類資料
                await SeedCategoriesAsync();

                // 種子管理員帳號
                await SeedAdminUserAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
            }
        }

        private async Task SeedAdminUserAsync()
        {
            // 檢查是否已有管理員
            if (await _context.Users.AnyAsync(u => u.IsAdmin))
            {
                _logger.LogInformation("Admin user already exists, skipping seed");
                return;
            }

            // 從設定讀取預設管理員資訊
            var adminEmail = _configuration["DefaultAdmin:Email"] ?? "admin@badminton-forum.com";
            var adminPassword = _configuration["DefaultAdmin:Password"] ?? "Admin123456!";

            // 檢查是否該 email 已存在但不是管理員
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
            if (existingUser != null)
            {
                // 如果用戶存在但不是管理員，將其設為管理員
                existingUser.IsAdmin = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Upgraded existing user to admin: {adminEmail}");
                return;
            }

            var admin = new User
            {
                Username = adminEmail.Split('@')[0],
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                IsAdmin = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Provider = "Local"  // 注意：應該是大寫 "Local" 以符合現有程式碼
            };

            _context.Users.Add(admin);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created default admin user: {adminEmail}");
        }

        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Categories already exist, skipping seed");
                return;
            }

            var categories = new[]
            {
                new Category { Name = "綜合討論區", Description = "羽毛球相關的一般討論", Icon = "💬", DisplayOrder = 1 },
                new Category { Name = "技術交流區", Description = "技術分享與教學討論", Icon = "🏸", DisplayOrder = 2 },
                new Category { Name = "裝備討論區", Description = "球拍、球鞋、裝備評測與推薦", Icon = "🎾", DisplayOrder = 3 },
                new Category { Name = "賽事專區", Description = "國內外賽事討論與轉播", Icon = "🏆", DisplayOrder = 4 },
                new Category { Name = "地區球友會", Description = "各地區球友交流與約球", Icon = "📍", DisplayOrder = 5 }
            };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {categories.Length} categories");
        }
    }
}