namespace AuthenticationService.DTOs
{
    public class SuperAdminLoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public SuperAdminInfo AdminInfo { get; set; } = new();
    }

    public class SuperAdminInfo
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Role { get; set; } = "SuperAdmin";
        public DateTime? LastLoginAt { get; set; }
    }
}