using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using UserService.Models;
using EntityState = UserService.Models.EntityState;
using EntityType = UserService.Enums.EntityType;

public abstract class EntityBaseDomain
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }

    [NotMapped]
    public EntityType Type { get; set; } = EntityType.None;

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

    [ConcurrencyCheck]
    public uint RowVersion { get; set; }

    [Column(TypeName = "jsonb")]
    public string? JsonData { get; set; }

    public void SerializeComplexData()
    {
        var complexData = new
        {
            Mapping = Mapping,
            UniqueIdentifiers = UniqueIdentifiers,
            States = States,
            Participations = Participations,
            Attachments = Attachments,
            Properties = Properties
        };
        JsonData = JsonSerializer.Serialize(complexData);
    }

    public void DeserializeComplexData()
    {
        if (!string.IsNullOrEmpty(JsonData))
        {
            var complexData = JsonSerializer.Deserialize<ComplexEntityDataDto>(JsonData);
            if (complexData != null)
            {
                Mapping = complexData.Mapping;
                UniqueIdentifiers = complexData.UniqueIdentifiers ?? new List<EntityUniqueIdentifier>();
                States = complexData.States ?? new List<EntityState>();
                Participations = complexData.Participations ?? new List<EntityParticipation>();
                Attachments = complexData.Attachments ?? new List<EntityAttachment>();
                Properties = complexData.Properties ?? new List<EntityProperty>();
            }
        }
    }

    public class ComplexEntityDataDto
    {
        public Guid Mapping { get; set; }
        public List<EntityUniqueIdentifier>? UniqueIdentifiers { get; set; }
        public List<EntityState>? States { get; set; }
        public List<EntityParticipation>? Participations { get; set; }
        public List<EntityAttachment>? Attachments { get; set; }
        public List<EntityProperty>? Properties { get; set; }
    }
}
