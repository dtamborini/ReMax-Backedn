using System.ComponentModel.DataAnnotations.Schema;
using AttachmentService.Models.MappingDao;

namespace AttachmentService.Models
{
    public class EntityProperty : EntityPropertyMapping
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public List<EntityDate>? Dates { get; set; }

    }
}
