using MappingService.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using EntityType = MappingService.Enums.EntityType;

public abstract class EntityBaseMapping
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }

    public required EntityType Type { get; set; }

    [NotMapped]
    public List<EntityAttachment> Attachments { get; set; } = new List<EntityAttachment>();

    [NotMapped]
    public List<EntityPropertyMapping> Properties { get; set; } = new List<EntityPropertyMapping>();

    public uint RowVersion { get; set; }

    [JsonIgnore]
    [Column(TypeName = "jsonb")]
    public string MappingJsonData { get; set; } = "{}";

    public void SerializeComplexData()
    {
        var complexData = new ComplexMappingDataDto
        {
            Type = Type,
            Attachments = Attachments,
            Properties = Properties
        };
        MappingJsonData = JsonSerializer.Serialize(complexData);
    }

    public void DeserializeComplexData()
    {
        if (!string.IsNullOrEmpty(MappingJsonData) && MappingJsonData != "{}")
        {
            var complexData = JsonSerializer.Deserialize<ComplexMappingDataDto>(MappingJsonData);
            if (complexData != null)
            {
                Type = complexData.Type;
                Attachments = complexData.Attachments ?? new List<EntityAttachment>();
                Properties = complexData.Properties ?? new List<EntityPropertyMapping>();
            }
        }
    }

    protected class ComplexMappingDataDto
    {
        public EntityType Type { get; set; }
        public List<EntityAttachment>? Attachments { get; set; }
        public List<EntityPropertyMapping>? Properties { get; set; }
    }
}
