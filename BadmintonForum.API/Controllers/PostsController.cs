using BadmintonForum.API.DTOs;
using BadmintonForum.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BadmintonForum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = userIdClaim != null ? int.Parse(userIdClaim) : (int?)null;

            var (posts, totalCount) = await _postService.GetPostsAsync(page, pageSize, userId);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
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

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = userIdClaim != null ? int.Parse(userIdClaim) : (int?)null;

            var (posts, totalCount) = await _postService.SearchPostsAsync(keyword, page, pageSize, userId);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            Response.Headers.Append("X-Total-Pages", totalPages.ToString());

            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = userIdClaim != null ? int.Parse(userIdClaim) : (int?)null;

            var post = await _postService.GetPostByIdAsync(id, userId);
            if (post == null)
            {
                return NotFound();
            }

            // Increment view count
            await _postService.IncrementViewCountAsync(id);

            return Ok(post);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto createPostDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var postDto = await _postService.CreatePostAsync(createPostDto, userId);
            return CreatedAtAction(nameof(GetPost), new { id = postDto.Id }, postDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, UpdatePostDto updatePostDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await _postService.UpdatePostAsync(id, updatePostDto, userId);
            
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await _postService.DeletePostAsync(id, userId);
            
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikePost(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var (success, isLiked, likeCount) = await _postService.ToggleLikeAsync(id, userId);
            
            if (!success)
            {
                return NotFound();
            }

            return Ok(new { likeCount, isLiked });
        }
    }
}