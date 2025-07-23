using BadmintonForum.API.Data;
using BadmintonForum.API.DTOs;
using BadmintonForum.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BadmintonForum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserAdminDto>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {

            var query = _context.Users.OrderBy(u => u.Username);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserAdminDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    IsAdmin = u.IsAdmin,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt,
                    PostCount = u.Posts.Count,
                    ReplyCount = u.Replies.Count
                })
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", totalItems.ToString());
            Response.Headers.Append("X-Total-Pages", totalPages.ToString());

            return Ok(users);
        }

        [HttpPut("users/{id}/toggle-active")]
        public async Task<IActionResult> ToggleUserActive(int id)
        {

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent admin from deactivating themselves
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (user.Id == currentUserId)
            {
                return BadRequest("不能停用自己的帳號");
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { isActive = user.IsActive });
        }

        [HttpPut("users/{id}/toggle-admin")]
        public async Task<IActionResult> ToggleUserAdmin(int id)
        {

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent admin from removing their own admin rights
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (user.Id == currentUserId)
            {
                return BadRequest("不能移除自己的管理員權限");
            }

            user.IsAdmin = !user.IsAdmin;
            await _context.SaveChangesAsync();

            return Ok(new { isAdmin = user.IsAdmin });
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryAdminDto>>> GetCategoriesAdmin()
        {

            var categories = await _context.Categories
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CategoryAdminDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Icon = c.Icon,
                    DisplayOrder = c.DisplayOrder,
                    PostCount = c.Posts.Count
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost("categories")]
        public async Task<ActionResult<Category>> CreateCategory(CreateCategoryDto dto)
        {

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Icon = dto.Icon,
                DisplayOrder = dto.DisplayOrder
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoriesAdmin), new { id = category.Id }, category);
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
        {

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
                category.Name = dto.Name;

            if (dto.Description != null)
                category.Description = dto.Description;

            if (dto.Icon != null)
                category.Icon = dto.Icon;

            if (dto.DisplayOrder.HasValue)
                category.DisplayOrder = dto.DisplayOrder.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {

            var category = await _context.Categories
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Posts.Any())
            {
                return BadRequest("無法刪除含有文章的版塊");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("posts")]
        public async Task<ActionResult<IEnumerable<PostAdminDto>>> GetPostsAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {

            var query = _context.Posts.OrderByDescending(p => p.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostAdminDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Username,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    ReplyCount = p.Replies.Count,
                    IsPinned = p.IsPinned,
                    IsLocked = p.IsLocked,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", totalItems.ToString());
            Response.Headers.Append("X-Total-Pages", totalPages.ToString());

            return Ok(posts);
        }

        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> DeletePostAdmin(int id)
        {

            var post = await _context.Posts
                .Include(p => p.Replies)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("posts/{id}/move")]
        public async Task<IActionResult> MovePost(int id, [FromBody] MovePostDto dto)
        {

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var targetCategory = await _context.Categories.FindAsync(dto.TargetCategoryId);
            if (targetCategory == null)
            {
                return BadRequest("目標版塊不存在");
            }

            post.CategoryId = dto.TargetCategoryId;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("posts/{id}/toggle-pin")]
        public async Task<IActionResult> TogglePostPin(int id)
        {

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.IsPinned = !post.IsPinned;
            await _context.SaveChangesAsync();

            return Ok(new { isPinned = post.IsPinned });
        }

        [HttpPut("posts/{id}/toggle-lock")]
        public async Task<IActionResult> TogglePostLock(int id)
        {

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.IsLocked = !post.IsLocked;
            await _context.SaveChangesAsync();

            return Ok(new { isLocked = post.IsLocked });
        }
    }
}