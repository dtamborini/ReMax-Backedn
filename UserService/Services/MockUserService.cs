namespace UserService.Services
{
    public interface IMockUserService
    {
        Task<MockUser?> ValidateUserAsync(string username, string password);
        Task<List<MockUser>> GetAllUsersAsync();
    }

    public class MockUser
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class MockUserService : IMockUserService
    {
        private readonly ILogger<MockUserService> _logger;
        private static readonly List<MockUser> _users = new()
        {
            new MockUser
            {
                Id = "user-001",
                Username = "admin",
                Email = "admin@remax.com",
                Password = "admin123",
                Roles = new List<string> { "admin", "user" },
                FirstName = "Mario",
                LastName = "Rossi",
                IsActive = true
            },
            new MockUser
            {
                Id = "user-002", 
                Username = "manager",
                Email = "manager@remax.com",
                Password = "manager123",
                Roles = new List<string> { "manager", "user" },
                FirstName = "Luigi",
                LastName = "Verdi",
                IsActive = true
            },
            new MockUser
            {
                Id = "user-003",
                Username = "user",
                Email = "user@remax.com", 
                Password = "user123",
                Roles = new List<string> { "user" },
                FirstName = "Giuseppe",
                LastName = "Bianchi",
                IsActive = true
            },
            new MockUser
            {
                Id = "user-004",
                Username = "testuser",
                Email = "testuser@remax.com",
                Password = "testpassword",
                Roles = new List<string> { "user", "tester" },
                FirstName = "Test",
                LastName = "User",
                IsActive = true
            }
        };

        public MockUserService(ILogger<MockUserService> logger)
        {
            _logger = logger;
        }

        public async Task<MockUser?> ValidateUserAsync(string username, string password)
        {
            await Task.Delay(100); // Simula una chiamata al database

            var user = _users.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                u.Password == password && 
                u.IsActive);

            if (user != null)
            {
                _logger.LogInformation("User validation successful for: {Username}", username);
            }
            else
            {
                _logger.LogWarning("User validation failed for: {Username}", username);
            }

            return user;
        }

        public async Task<List<MockUser>> GetAllUsersAsync()
        {
            await Task.Delay(50);
            return _users.Where(u => u.IsActive).ToList();
        }
    }
}