using System.ComponentModel.DataAnnotations.Schema;

namespace WorkOrderService.Models
{
    public class EntityProperty
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Type { get; set; }
        public List<LocalizedTitle> Title { get; set; } = new List<LocalizedTitle>();

        [NotMapped]
        public dynamic? Attributes { get; set; }
        public List<EntityDate>? Dates { get; set; }
        public string? Value { get; set; }
        public List<EntityProperty> Properties { get; set; } = new List<EntityProperty>();
    }
}
