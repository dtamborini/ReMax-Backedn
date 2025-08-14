namespace RemaxManagement.Shared.Auth
{
    public static class JwtConstants
    {
        public static class ClaimTypes
        {
            public const string Username = "username";
            public const string Email = "email";
            public const string FullName = "fullName";
            public const string Role = "role";
        }
        
        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Supplier = "Supplier";
            public const string Resident = "Resident";
        }
    }
}