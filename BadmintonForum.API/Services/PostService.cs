using BadmintonForum.API.Data;
using BadmintonForum.API.DTOs;
using BadmintonForum.API.Models;
using BadmintonForum.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BadmintonForum.API.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<PostDto> posts, int totalCount)> GetPostsAsync(int page, int pageSize, int? userId = null)
        {
            var query = _context.Posts
                .OrderByDescending(p => p.CreatedAt);

            var totalCount = await query.CountAsync();

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
                    IsDeleted = p.IsDeleted,
                    DeletedAt = p.DeletedAt,
                    IsLiked = userId.HasValue && userId > 0 && p.PostLikes.Any(pl => pl.UserId == userId),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return (posts, totalCount);
        }

        public async Task<(IEnumerable<PostDto> posts, int totalCount)> SearchPostsAsync(string keyword, int page, int pageSize, int? userId = null)
        {
            var query = _context.Posts
                .Where(p => p.Title.Contains(keyword) || p.Content.Contains(keyword))
                .OrderByDescending(p => p.CreatedAt);

            var totalCount = await query.CountAsync();

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
                    IsDeleted = p.IsDeleted,
                    DeletedAt = p.DeletedAt,
                    IsLiked = userId.HasValue && userId > 0 && p.PostLikes.Any(pl => pl.UserId == userId),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return (posts, totalCount);
        }

        public async Task<PostDto?> GetPostByIdAsync(int id, int? userId = null)
        {
            return await _context.Posts
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
                    IsDeleted = p.IsDeleted,
                    DeletedAt = p.DeletedAt,
                    IsLiked = userId.HasValue && userId > 0 && p.PostLikes.Any(pl => pl.UserId == userId),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, int userId)
        {
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

            // 重新查詢以取得完整資料
            var postDto = await GetPostByIdAsync(post.Id, userId);
            return postDto!;
        }

        public async Task<bool> UpdatePostAsync(int id, UpdatePostDto updatePostDto, int userId)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.AuthorId != userId)
            {
                return false;
            }

            // 防止編輯已刪除的文章
            if (post.IsDeleted)
            {
                return false;
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

            return true;
        }

        public async Task<bool> DeletePostAsync(int id, int userId)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.AuthorId != userId)
            {
                return false;
            }

            // 軟刪除：標記為已刪除而非真正刪除
            post.IsDeleted = true;
            post.DeletedAt = DateTime.UtcNow;
            
            // 清空內容但保留結構
            post.Content = "[此文章已被作者刪除]";
            // 保留標題以維持討論脈絡
            // post.Title = "[已刪除]";  // 可選：也可以保留原標題
            
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<(bool success, bool isLiked, int likeCount)> ToggleLikeAsync(int postId, int userId)
        {
            // 檢查文章是否存在
            var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
            {
                return (false, false, 0);
            }

            // 檢查是否已經點讚
            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

            bool isLiked;
            
            if (existingLike != null)
            {
                // 取消讚
                _context.PostLikes.Remove(existingLike);
                await _context.Posts
                    .Where(p => p.Id == postId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(p => p.LikeCount, p => p.LikeCount - 1));
                isLiked = false;
            }
            else
            {
                // 新增點讚
                var postLike = new PostLike
                {
                    PostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostLikes.Add(postLike);
                await _context.Posts
                    .Where(p => p.Id == postId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(p => p.LikeCount, p => p.LikeCount + 1));
                isLiked = true;
            }

            await _context.SaveChangesAsync();

            // 取得最新的點讚數
            var likeCount = await _context.Posts
                .Where(p => p.Id == postId)
                .Select(p => p.LikeCount)
                .FirstOrDefaultAsync();

            return (true, isLiked, likeCount);
        }

        public async Task IncrementViewCountAsync(int postId)
        {
            await _context.Posts
                .Where(p => p.Id == postId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1));
        }
    }
}