using BadmintonForum.API.Data;
using BadmintonForum.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BadmintonForum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<ProfileDto>> GetProfile(string username)
        {
            var profile = await _context.Users
                .Where(u => u.Username == username)
                .Select(u => new ProfileDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    Bio = u.Bio,
                    CreatedAt = u.CreatedAt,
                    PostCount = u.Posts.Count,
                    ReplyCount = u.Replies.Count,
                    PlayingStyle = u.PlayingStyle,
                    YearsOfExperience = u.YearsOfExperience,
                    Signature = u.Signature
                })
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpGet("{username}/posts")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetUserPosts(string username, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            var query = _context.Posts
                .Where(p => p.AuthorId == user.Id)
                .OrderByDescending(p => p.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Username,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    ReplyCount = p.Replies.Count,
                    IsPinned = p.IsPinned,
                    IsLocked = p.IsLocked,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", totalItems.ToString());
            Response.Headers.Append("X-Total-Pages", totalPages.ToString());

            return Ok(posts);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(updateProfileDto.Email))
            {
                // Check if email is already taken
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == updateProfileDto.Email && u.Id != userId);

                if (emailExists)
                {
                    return BadRequest(new { message = "電子郵件已被使用" });
                }

                user.Email = updateProfileDto.Email;
            }

            if (updateProfileDto.Bio != null)
            {
                user.Bio = updateProfileDto.Bio;
            }

            if (updateProfileDto.Avatar != null)
            {
                user.Avatar = updateProfileDto.Avatar;
            }

            if (updateProfileDto.PlayingStyle != null)
            {
                user.PlayingStyle = updateProfileDto.PlayingStyle;
            }

            if (updateProfileDto.YearsOfExperience.HasValue)
            {
                user.YearsOfExperience = updateProfileDto.YearsOfExperience;
            }

            if (updateProfileDto.Signature != null)
            {
                user.Signature = updateProfileDto.Signature;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("change-REMOVED")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Verify current REMOVED
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "當前密碼不正確" });
            }

            // Update REMOVED
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "密碼已成功更改" });
        }
    }
}