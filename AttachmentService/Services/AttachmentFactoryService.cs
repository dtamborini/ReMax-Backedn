using AttachmentService.Interfaces;
using AttachmentService.Models;
using AttachmentService.Models.MappingDao;
using AttachmentService.Enums;

namespace AttachmentService.Services
{
    public class AttachmentFactoryService : IAttachmentFactoryService
    {
        public Attachment CreateAttachment(
            Guid attachmentGuid,
            string fileName,
            EntityMapping entityMapping,
            Guid buildingGuid,
            Guid userUuid,
            string fileContentType
        )
        {
            var participation = new EntityParticipation
            {
                User = userUuid,
                Timestamp = DateTime.UtcNow,
            };

            // Entity
            var attachment = new Attachment
            {
                Guid = attachmentGuid,
                BuildingGuid = buildingGuid,
                Name = fileName,
                Mapping = entityMapping.Guid,
                Properties = PropertyMapper.MapProperties(entityMapping.Properties, participation),
            };

            // Identifiers
            attachment.UniqueIdentifiers.Add(new EntityUniqueIdentifier
            {
                Type = UniqueIdentifierType.QR,
                Value = attachment.Guid.ToString()
            });

            // Attachments
            attachment.Attachments.Add(new EntityAttachment
            {
                Guid = attachmentGuid,
                Name = fileName,
                Content = fileContentType,
                FileName = fileName,
                Dates = new List<EntityDate> {
                    new EntityDate
                    {
                        DateType = DateType.Created,
                        User = participation
                    },
                },
            });

            // Participations
            attachment.Participations.Add(new EntityParticipation
            {
                User = userUuid,
                Timestamp = DateTime.UtcNow,
            });

            // States
            attachment.States.Add(new EntityState
            {
                Value = DateType.Created,
                Date = DateTime.UtcNow,
            });

            return attachment;
        }
    }

    public static class PropertyMapper
    {
        public static List<EntityProperty> MapProperties(
            List<EntityPropertyMapping> entityPropertyMappings,
            EntityParticipation participation)
        {
            if (entityPropertyMappings == null)
            {
                return new List<EntityProperty>();
            }

            return entityPropertyMappings.Select(mapping => new EntityProperty
            {
                Name = mapping.Name,
                Type = mapping.Type,
                Title = mapping.Title,
                Attributes = mapping.Attributes,
                Value = mapping.Value,
                Properties = MapProperties(mapping.Properties, participation),
                Dates = new List<EntityDate>
                {
                    new EntityDate
                    {
                        DateType = DateType.Created,
                        User = participation
                    }
                },
                HashCode = mapping.HashCode,
            }).ToList();
        }
    }
}