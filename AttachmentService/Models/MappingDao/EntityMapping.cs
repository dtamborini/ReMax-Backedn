using AttachmentService.Enums;

namespace AttachmentService.Models.MappingDao
{
    public class EntityMapping
    {
        public required Guid Guid { get; set; }
        public required string Name { get; set; }
        public required EntityType Type { get; set; }
        public List<EntityAttachment> Attachments { get; set; } = new List<EntityAttachment>();
        public List<EntityPropertyMapping> Properties { get; set; } = new List<EntityPropertyMapping>();
        public bool IsActive { get; set; } = false;
        public required int Version { get; set; }
        public uint RowVersion { get; set; }
    }
}