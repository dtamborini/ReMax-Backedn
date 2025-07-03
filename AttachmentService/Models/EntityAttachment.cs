namespace AttachmentService.Models
{
    public class EntityAttachment
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public required string Name { get; init; }
        public string? Description { get; set; }
        public required string Content { get; init; }
        public required string FileName { get; init; }
        public Guid PropertyReference { get; init; }
        public List<EntityDate> Dates { get; set; } = new List<EntityDate>();
    }
}
