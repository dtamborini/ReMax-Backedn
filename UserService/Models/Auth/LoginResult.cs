namespace UserService.Models.Auth
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
}