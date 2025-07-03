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
                Properties = entityMapping.Properties.Select(mapping => new EntityProperty
                {
                    Type = mapping.Type,
                    Name = mapping.Name,
                    HashCode = mapping.HashCode,
                }).ToList(),
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
}