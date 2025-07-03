using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingService.Models
{
    public class BuildingDto
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }

        [NotMapped]
        public required Guid Mapping { get; set; }

        [NotMapped]
        public List<EntityUniqueIdentifier> UniqueIdentifiers { get; set; } = new List<EntityUniqueIdentifier>();

        [NotMapped]
        public List<EntityState> States { get; set; } = new List<EntityState>();

        [NotMapped]
        public List<EntityParticipation> Participations { get; set; } = new List<EntityParticipation>();

        [NotMapped]
        public List<EntityAttachment> Attachments { get; set; } = new List<EntityAttachment>();

        [NotMapped]
        public List<EntityProperty> Properties { get; set; } = new List<EntityProperty>();
        public String? RowVersion { get; set; }
    }
}