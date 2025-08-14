namespace RemaxManagement.Shared.Options
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        
        public string Key { get; set; } = string.Empty;
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int AccessTokenExpiration { get; set; } = 60; // minuti
        public int RefreshTokenExpiration { get; set; } = 10080; // 7 giorni in minuti
    }
}