﻿using BuildingService.Enums;

namespace BuildingService.Models
{
    public class EntityProperty
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public required string Name { get; init; }
        public required PropertyType Type { get; init; }
        public List<LocalizedTitle> Title { get; init; } = new List<LocalizedTitle>();
        public dynamic? Attributes { get; init; }
        public dynamic? Value { get; set; }
        public List<EntityProperty>? Properties { get; set; }
        public List<EntityDate>? Dates { get; set; }
    }
}
