using BuildingService.Enums;
using BuildingService.Interfaces;
using BuildingService.Models;
using BuildingService.Models.MappingDao;
using System.Reflection;

namespace BuildingService.Services
{
    public class BuildingFactoryService : IBuildingFactoryService
    {
        public Building MapBuildingDto(
            BuildingDto buildingDto,
            EntityMapping entityMapping,
            Guid userUuid
        )
        {
            var participation = new EntityParticipation
            {
                User = userUuid,
                Timestamp = DateTime.UtcNow,
            };

            // Entity
            var building = new Building
            {
                Guid = buildingDto.Guid,
                Name = buildingDto.Name,
                Mapping = entityMapping.Guid,
                Properties = PropertyMapper.MapProperties(
                    entityMapping.Properties,
                    participation,
                    buildingDto
                    ),
            };

            // Identifiers
            building.UniqueIdentifiers.Add(new EntityUniqueIdentifier
            {
                Type = UniqueIdentifierType.QR,
                Value = buildingDto.Guid.ToString()
            });

            // Participations
            //building.Participations.Add(new EntityParticipation
            //{
            //    User = userUuid,
            //    Timestamp = DateTime.UtcNow,
            //});

            // States
            //attachment.States.Add(new EntityState
            //{
            //    Value = DateType.Created,
            //    Date = DateTime.UtcNow,
            //});

            return building;
        }
    }

    public static class PropertyMapper
    {
        public static List<EntityProperty> MapProperties(
        List<EntityPropertyMapping> entityPropertyMappings,
        EntityParticipation participation,
        object currentDtoObject)
        {
            if (entityPropertyMappings == null || currentDtoObject == null)
            {
                return new List<EntityProperty>();
            }

            Type currentDtoType = currentDtoObject.GetType();

            return entityPropertyMappings.Select(mapping =>
            {
                object? propertyValue = null;
                object? nestedDtoObject = null;

                PropertyInfo? propertyInfo = currentDtoType.GetProperty(mapping.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (propertyInfo != null && propertyInfo.CanRead)
                {
                    object? value = propertyInfo.GetValue(currentDtoObject);

                    if (mapping.Properties != null && mapping.Properties.Any())
                    {
                        nestedDtoObject = value;
                        propertyValue = null;
                    }
                    else
                    {
                        propertyValue = value;
                    }
                }

                return new EntityProperty
                {
                    Name = mapping.Name,
                    Type = mapping.Type,
                    Title = mapping.Title,
                    Value = propertyValue,
                    Properties = MapProperties(mapping.Properties, participation, nestedDtoObject)
                };
            }).ToList();
        }
        
        public static List<EntityProperty> MapBuildingProperties(
            List<EntityPropertyMapping> topLevelMappings,
            EntityParticipation participation,
            BuildingDto buildingDto)
        {
            return MapProperties(topLevelMappings, participation, buildingDto);
        }
    }
}