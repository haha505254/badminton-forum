namespace BadmintonForum.API.DTOs
{
    public class UserAdminDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public int PostCount { get; set; }
        public int ReplyCount { get; set; }
    }

    public class CategoryAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public int PostCount { get; set; }
    }

    public class PostAdminDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        public bool IsPinned { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateCategoryDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int? DisplayOrder { get; set; }
    }

    public class MovePostDto
    {
        public int TargetCategoryId { get; set; }
    }
}