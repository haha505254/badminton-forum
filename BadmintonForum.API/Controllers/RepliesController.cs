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
            // 包含已刪除的回覆（為了保持樹狀結構）
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
                    Content = r.IsDeleted ? "[此留言已被刪除]" : r.Content,
                    PostId = r.PostId,
                    AuthorId = r.IsDeleted ? 0 : r.AuthorId,
                    AuthorName = r.IsDeleted ? "[已刪除]" : r.Author.Username,
                    ParentReplyId = r.ParentReplyId,
                    LikeCount = r.LikeCount,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    IsDeleted = r.IsDeleted,
                    DeletedAt = r.DeletedAt
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
                    UpdatedAt = r.UpdatedAt,
                    IsDeleted = false,
                    DeletedAt = null
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetReplies), new { postId = postId }, replyDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReply(int postId, int id, [FromBody] UpdateReplyDto updateReplyDto)
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
            
            if (reply.IsDeleted)
            {
                return BadRequest("無法編輯已刪除的回覆");
            }

            reply.Content = updateReplyDto.Content;
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
                .Include(r => r.ChildReplies)
                .FirstOrDefaultAsync(r => r.Id == id && r.PostId == postId);

            if (reply == null)
            {
                return NotFound();
            }

            if (reply.AuthorId != userId)
            {
                return Forbid();
            }
            
            if (reply.IsDeleted)
            {
                return BadRequest("此回覆已經被刪除");
            }

            // 軟刪除：標記為已刪除而非真的刪除
            reply.IsDeleted = true;
            reply.DeletedAt = DateTime.UtcNow;
            reply.Content = "[此留言已被刪除]";  // 清空內容
            
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