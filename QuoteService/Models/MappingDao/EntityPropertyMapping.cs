using QuoteService.Enums;

namespace QuoteService.Models.MappingDao
{
    public class EntityPropertyMapping
    {
        public required string Name { get; init; }
        public required PropertyType Type { get; init; }
        public List<LocalizedTitle> Title { get; init; } = new List<LocalizedTitle>();
        public dynamic? Attributes { get; init; }
        public string? Value { get; init; }
        public List<EntityPropertyMapping> Properties { get; init; } = new List<EntityPropertyMapping>();
        public required int HashCode { get; init; }
    }
}
