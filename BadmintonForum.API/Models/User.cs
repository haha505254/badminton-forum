using System.ComponentModel.DataAnnotations;

namespace BadmintonForum.API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? Avatar { get; set; }

        public string? Bio { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsAdmin { get; set; } = false;

        // Badminton-specific fields
        public string? PlayingStyle { get; set; } // 攻擊型、防守型、全能型
        
        public int? YearsOfExperience { get; set; }
        
        public string? Signature { get; set; } // 簽名檔

        // Password Reset
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Navigation properties
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();
    }
}