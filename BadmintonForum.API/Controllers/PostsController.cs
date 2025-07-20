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
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var query = _context.Posts
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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PostDto>>> SearchPosts([FromQuery] string keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required");
            }

            var query = _context.Posts
                .Where(p => p.Title.Contains(keyword) || p.Content.Contains(keyword))
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

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var post = await _context.Posts
                .Where(p => p.Id == id)
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
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            // Increment view count
            await _context.Posts
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1));

            return Ok(post);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto createPostDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var post = new Post
            {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                CategoryId = createPostDto.CategoryId,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var postDto = await _context.Posts
                .Where(p => p.Id == post.Id)
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
                    ReplyCount = 0,
                    IsPinned = p.IsPinned,
                    IsLocked = p.IsLocked,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, postDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, UpdatePostDto updatePostDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.AuthorId != userId)
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(updatePostDto.Title))
            {
                post.Title = updatePostDto.Title;
            }

            if (!string.IsNullOrWhiteSpace(updatePostDto.Content))
            {
                post.Content = updatePostDto.Content;
            }

            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.AuthorId != userId)
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.LikeCount++;
            await _context.SaveChangesAsync();

            return Ok(new { likeCount = post.LikeCount });
        }
    }
}