namespace BadmintonForum.API.DTOs
{
    public class ReplyDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int? ParentReplyId { get; set; }
        public int LikeCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class CreateReplyDto
    {
        public string Content { get; set; } = string.Empty;
        public int? ParentReplyId { get; set; }
    }
    
    public class UpdateReplyDto
    {
        public string Content { get; set; } = string.Empty;
    }
}