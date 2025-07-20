namespace BadmintonForum.API.DTOs
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostCount { get; set; }
        public int ReplyCount { get; set; }
        public string? PlayingStyle { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Signature { get; set; }
    }

    public class UpdateProfileDto
    {
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? Avatar { get; set; }
        public string? PlayingStyle { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Signature { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}