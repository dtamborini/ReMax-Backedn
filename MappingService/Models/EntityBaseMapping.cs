using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using UserService.Models;
using EntityState = UserService.Models.EntityState;
using EntityType = UserService.Enums.EntityType;

public abstract class EntityBaseMapping
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }

    public EntityType Type { get; set; } = EntityType.None;

    [NotMapped]
    public List<EntityState> States { get; set; } = new List<EntityState>();

    [NotMapped]
    public List<EntityAttachment> Attachments { get; set; } = new List<EntityAttachment>();

    [NotMapped]
    public List<EntityProperty> Properties { get; set; } = new List<EntityProperty>();

    [ConcurrencyCheck]
    public uint RowVersion { get; set; }

    [Column(TypeName = "jsonb")]
    public string MappingJsonData { get; set; } = "{}";

    protected void SerializeComplexData()
    {
        var complexData = new ComplexMappingDataDto
        {
            Type = Type,
            States = States,
            Attachments = Attachments,
            Properties = Properties
        };
        MappingJsonData = JsonSerializer.Serialize(complexData);
    }

    protected void DeserializeComplexData()
    {
        if (!string.IsNullOrEmpty(MappingJsonData) && MappingJsonData != "{}")
        {
            var complexData = JsonSerializer.Deserialize<ComplexMappingDataDto>(MappingJsonData);
            if (complexData != null)
            {
                Type = complexData.Type;
                States = complexData.States ?? new List<EntityState>();
                Attachments = complexData.Attachments ?? new List<EntityAttachment>();
                Properties = complexData.Properties ?? new List<EntityProperty>();
            }
        }
    }

    protected class ComplexMappingDataDto
    {
        public EntityType Type { get; set; }
        public List<EntityState>? States { get; set; }
        public List<EntityAttachment>? Attachments { get; set; }
        public List<EntityProperty>? Properties { get; set; }
    }
}
