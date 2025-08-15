namespace BadmintonForum.API.DTOs
{
    public class ReplyAdminDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int PostId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        // Parent reply information
        public int? ParentReplyId { get; set; }
        public string? ParentReplyContent { get; set; }
        public string? ParentReplyAuthorName { get; set; }
        public int? ParentReplyAuthorId { get; set; }
        public bool? ParentReplyIsDeleted { get; set; }
        public DateTime? ParentReplyDeletedAt { get; set; }
    }

    public class BatchDeleteDto
    {
        public List<int> Ids { get; set; } = new List<int>();
    }
}