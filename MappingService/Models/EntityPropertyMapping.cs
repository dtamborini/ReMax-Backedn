using MappingService.Enums;
using System.Text.Json;

namespace MappingService.Models
{
    public class EntityPropertyMapping
    {
        private readonly int _hashCode;

        public required string Name { get; init; }
        public required PropertyType Type { get; init; }
        public List<LocalizedTitle> Title { get; init; } = new List<LocalizedTitle>();
        public dynamic? Attributes { get; init; }
        public string? Value { get; init; }
        public List<EntityPropertyMapping> Properties { get; init; } = new List<EntityPropertyMapping>();

        public EntityPropertyMapping(
            string name,
            PropertyType type,
            List<LocalizedTitle>? title = null,
            dynamic? attributes = null,
            string? value = null,
            List<EntityPropertyMapping>? properties = null)
        {
            Name = name;
            Type = type;
            Title = title ?? new List<LocalizedTitle>();
            Attributes = attributes;
            Value = value;
            Properties = properties ?? new List<EntityPropertyMapping>();

            _hashCode = CalculateHashCode();
        }

        public int HashCode => _hashCode;

        private int CalculateHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(Type);

            foreach (var item in Title)
            {
                hash.Add(item);
            }

            string? attributesJson = Attributes != null ? JsonSerializer.Serialize(Attributes) : null;
            hash.Add(attributesJson);

            hash.Add(Value);
            foreach (var item in Properties)
            {
                hash.Add(item); 
            }

            return hash.ToHashCode();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not EntityPropertyMapping other)
            {
                return false;
            }

            bool titlesEqual = Title.Count == other.Title.Count &&
                               !Title.Except(other.Title).Any() &&
                               !other.Title.Except(Title).Any();

            bool propertiesEqual = Properties.Count == other.Properties.Count &&
                                   !Properties.Except(other.Properties).Any() &&
                                   !other.Properties.Except(Properties).Any();

            string? thisAttributesJson = Attributes != null ? System.Text.Json.JsonSerializer.Serialize(Attributes) : null;
            string? otherAttributesJson = other.Attributes != null ? System.Text.Json.JsonSerializer.Serialize(other.Attributes) : null;


            return Name == other.Name &&
                   Type == other.Type &&
                   titlesEqual &&
                   thisAttributesJson == otherAttributesJson &&
                   Value == other.Value &&
                   propertiesEqual;
        }
    }
}
