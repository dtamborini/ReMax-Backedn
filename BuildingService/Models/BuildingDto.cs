namespace BuildingService.Models
{
    public class BuildingDto
    {
        public Guid Guid { get; init; } = Guid.NewGuid();
        public required string Name { get; init; }
        public required string Address { get; init; }
        public required string TelephoneNumber { get; init; }
        public required BuildingEmailDto Emails { get; init; }
        public required string Cf { get; init; }
        
    }

    public class BuildingEmailDto
    {
        public required string Email { get; init; }
        public required string Pec { get; init; }
    }
}