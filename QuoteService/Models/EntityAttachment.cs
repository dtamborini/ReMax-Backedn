namespace QuoteService.Models
{
    public class EntityAttachment
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public required string FileName { get; set; }
        public Guid PropertyReference { get; set; }
    }
}
