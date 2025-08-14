using System.ComponentModel.DataAnnotations;

namespace BadmintonForum.API.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public int AuthorId { get; set; }

        public int CategoryId { get; set; }

        public int ViewCount { get; set; } = 0;

        public int LikeCount { get; set; } = 0;

        public bool IsPinned { get; set; } = false;

        public bool IsLocked { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual User Author { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();
        public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    }
}