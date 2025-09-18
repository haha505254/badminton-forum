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
                .Where(p => !p.IsDeleted)  // 過濾已刪除的文章
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
                .Where(p => !p.IsDeleted && (p.Title.Contains(keyword) || p.Content.Contains(keyword)))
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
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Replies)
                .Include(p => p.PostLikes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return null;

            // 處理已刪除文章
            if (post.IsDeleted)
            {
                // 檢查是否為作者本人
                bool isAuthor = userId.HasValue && post.AuthorId == userId.Value;
                
                if (isAuthor)
                {
                    // 作者看到完整內容
                    return new PostDto
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Content = post.Content,  // 保留原始內容
                        AuthorId = post.AuthorId,
                        AuthorName = post.Author.Username,
                        CategoryId = post.CategoryId,
                        CategoryName = post.Category.Name,
                        ViewCount = post.ViewCount,
                        LikeCount = post.LikeCount,
                        ReplyCount = post.Replies.Count(r => !r.IsDeleted),
                        IsPinned = post.IsPinned,
                        IsLocked = post.IsLocked,
                        IsDeleted = true,
                        DeletedAt = post.DeletedAt,
                        IsLiked = false,
                        CreatedAt = post.CreatedAt,
                        UpdatedAt = post.UpdatedAt
                    };
                }

                // 檢查是否有未刪除的回覆
                bool hasActiveReplies = post.Replies.Any(r => !r.IsDeleted);
                
                if (hasActiveReplies)
                {
                    // 有回覆：顯示討論串框架
                    return new PostDto
                    {
                        Id = post.Id,
                        Title = post.Title,  // 保留標題
                        Content = "[此文章已被作者刪除]",
                        AuthorId = 0,
                        AuthorName = "[已刪除]",
                        CategoryId = post.CategoryId,
                        CategoryName = post.Category.Name,
                        ViewCount = 0,
                        LikeCount = 0,
                        ReplyCount = post.Replies.Count(r => !r.IsDeleted),
                        IsPinned = false,
                        IsLocked = true,  // 已刪除文章視為鎖定
                        IsDeleted = true,
                        DeletedAt = post.DeletedAt,
                        IsLiked = false,
                        CreatedAt = post.CreatedAt,
                        UpdatedAt = post.UpdatedAt
                    };
                }
                
                // 無回覆：不顯示（404）
                return null;
            }

            // 正常文章處理
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.Username,
                CategoryId = post.CategoryId,
                CategoryName = post.Category.Name,
                ViewCount = post.ViewCount,
                LikeCount = post.LikeCount,
                ReplyCount = post.Replies.Count(r => !r.IsDeleted),
                IsPinned = post.IsPinned,
                IsLocked = post.IsLocked,
                IsDeleted = false,
                DeletedAt = null,
                IsLiked = userId.HasValue && userId > 0 && post.PostLikes.Any(pl => pl.UserId == userId),
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
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
            
            // 保留原始內容，不做任何修改
            // 前端會根據 IsDeleted 標記決定是否顯示內容
            
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