namespace MappingService.Models
{
    public class EntityMapping : EntityBaseMapping
    {
        public bool IsActive { get; set; } = false;

        public required int Version { get; set; }
    }
}