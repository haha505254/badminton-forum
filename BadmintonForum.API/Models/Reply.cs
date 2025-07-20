using System.ComponentModel.DataAnnotations;

namespace BadmintonForum.API.Models
{
    public class Reply
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public int PostId { get; set; }

        public int AuthorId { get; set; }

        public int? ParentReplyId { get; set; }

        public int LikeCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Post Post { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
        public virtual Reply? ParentReply { get; set; }
        public virtual ICollection<Reply> ChildReplies { get; set; } = new List<Reply>();
    }
}