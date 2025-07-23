using BadmintonForum.API.DTOs;

namespace BadmintonForum.API.Services.Interfaces
{
    public interface IPostService
    {
        Task<(IEnumerable<PostDto> posts, int totalCount)> GetPostsAsync(int page, int pageSize, int? userId = null);
        Task<(IEnumerable<PostDto> posts, int totalCount)> SearchPostsAsync(string keyword, int page, int pageSize, int? userId = null);
        Task<PostDto?> GetPostByIdAsync(int id, int? userId = null);
        Task<PostDto> CreatePostAsync(CreatePostDto createPostDto, int userId);
        Task<bool> UpdatePostAsync(int id, UpdatePostDto updatePostDto, int userId);
        Task<bool> DeletePostAsync(int id, int userId);
        Task<(bool success, bool isLiked, int likeCount)> ToggleLikeAsync(int postId, int userId);
        Task IncrementViewCountAsync(int postId);
    }
}