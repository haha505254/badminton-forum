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
    [Route("api/posts/{postId}/[controller]")]
    public class RepliesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RepliesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReplyDto>>> GetReplies(int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var query = _context.Replies
                .Where(r => r.PostId == postId)
                .OrderBy(r => r.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var replies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReplyDto
                {
                    Id = r.Id,
                    Content = r.Content,
                    PostId = r.PostId,
                    AuthorId = r.AuthorId,
                    AuthorName = r.Author.Username,
                    ParentReplyId = r.ParentReplyId,
                    LikeCount = r.LikeCount,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", totalItems.ToString());
            Response.Headers.Append("X-Total-Pages", totalPages.ToString());

            return Ok(replies);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReplyDto>> CreateReply(int postId, CreateReplyDto createReplyDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound("文章不存在");
            }

            if (post.IsLocked)
            {
                return BadRequest("文章已鎖定，無法回覆");
            }

            var reply = new Reply
            {
                Content = createReplyDto.Content,
                PostId = postId,
                AuthorId = userId,
                ParentReplyId = createReplyDto.ParentReplyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Replies.Add(reply);
            await _context.SaveChangesAsync();

            var replyDto = await _context.Replies
                .Where(r => r.Id == reply.Id)
                .Select(r => new ReplyDto
                {
                    Id = r.Id,
                    Content = r.Content,
                    PostId = r.PostId,
                    AuthorId = r.AuthorId,
                    AuthorName = r.Author.Username,
                    ParentReplyId = r.ParentReplyId,
                    LikeCount = r.LikeCount,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetReplies), new { postId = postId }, replyDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReply(int postId, int id, [FromBody] string content)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var reply = await _context.Replies
                .FirstOrDefaultAsync(r => r.Id == id && r.PostId == postId);

            if (reply == null)
            {
                return NotFound();
            }

            if (reply.AuthorId != userId)
            {
                return Forbid();
            }

            reply.Content = content;
            reply.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReply(int postId, int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var reply = await _context.Replies
                .FirstOrDefaultAsync(r => r.Id == id && r.PostId == postId);

            if (reply == null)
            {
                return NotFound();
            }

            if (reply.AuthorId != userId)
            {
                return Forbid();
            }

            _context.Replies.Remove(reply);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikeReply(int postId, int id)
        {
            var reply = await _context.Replies
                .FirstOrDefaultAsync(r => r.Id == id && r.PostId == postId);

            if (reply == null)
            {
                return NotFound();
            }

            reply.LikeCount++;
            await _context.SaveChangesAsync();

            return Ok(new { likeCount = reply.LikeCount });
        }
    }
}