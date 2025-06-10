using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models
{
    public class EntityProperty
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Type { get; set; }
        public List<LocalizedTitle> Title { get; set; } = new List<LocalizedTitle>();

        [NotMapped]
        public dynamic? Attributes { get; set; }
        public Dictionary<string, EntityParticipation> Dates { get; set; } = new Dictionary<string, EntityParticipation>();
        public string? Value { get; set; }
        public List<EntityProperty> Properties { get; set; } = new List<EntityProperty>();
    }
}
