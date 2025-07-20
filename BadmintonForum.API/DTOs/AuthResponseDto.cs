namespace BadmintonForum.API.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAdmin { get; set; }
        public string? PlayingStyle { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Signature { get; set; }
    }
}