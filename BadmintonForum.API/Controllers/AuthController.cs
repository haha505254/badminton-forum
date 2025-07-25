using BadmintonForum.API.Data;
using BadmintonForum.API.DTOs;
using BadmintonForum.API.Models;
using BadmintonForum.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace BadmintonForum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ApplicationDbContext context,
            IJwtService jwtService,
            IConfiguration configuration,
            IEmailService emailService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { message = "此電子郵件已被註冊" });
            }

            // Use email as username
            var username = registerDto.Email;

            // Check if username already exists (for safety)
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                return BadRequest(new { message = "此帳號已存在" });
            }

            // Create new user
            var user = new User
            {
                Username = username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = _jwtService.GenerateToken(user);
            var expirationDays = Convert.ToDouble(_configuration["JwtSettings:ExpirationInDays"]);

            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Bio = user.Bio,
                    CreatedAt = user.CreatedAt,
                    IsAdmin = user.IsAdmin,
                    PlayingStyle = user.PlayingStyle,
                    YearsOfExperience = user.YearsOfExperience,
                    Signature = user.Signature
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // Find user by email (which is also the username)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email || u.Username == loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "帳號或密碼錯誤" });
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "密碼錯誤" });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "帳戶已被停用" });
            }

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Generate token
            var token = _jwtService.GenerateToken(user);
            var expirationDays = Convert.ToDouble(_configuration["JwtSettings:ExpirationInDays"]);

            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    Bio = user.Bio,
                    CreatedAt = user.CreatedAt,
                    IsAdmin = user.IsAdmin,
                    PlayingStyle = user.PlayingStyle,
                    YearsOfExperience = user.YearsOfExperience,
                    Signature = user.Signature
                }
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);

            if (user == null)
            {
                // 不要洩露用戶是否存在
                return Ok(new { message = "如果該電子郵件地址存在，我們已發送重置密碼的說明。" });
            }

            // 生成重置令牌
            var token = GeneratePasswordResetToken();
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(24); // 令牌有效期24小時

            await _context.SaveChangesAsync();

            try
            {
                // 發送重置密碼郵件
                await _emailService.SendPasswordResetEmailAsync(user.Email, user.Username, token);
                _logger.LogInformation($"Password reset email sent to {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {user.Email}");
                // 即使郵件發送失敗，也不要讓用戶知道
            }

            // 不要洩露任何資訊給用戶
            return Ok(new
            {
                message = "如果該電子郵件地址存在，我們已發送重置密碼的說明。請檢查您的郵箱。"
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == resetPasswordDto.Token &&
                u.PasswordResetTokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return BadRequest(new { message = "無效或過期的重置令牌" });
            }

            // 更新密碼
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "密碼已成功重置" });
        }

        private string GeneratePasswordResetToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}