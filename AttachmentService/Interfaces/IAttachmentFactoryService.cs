using AttachmentService.Models.MappingDao;
using AttachmentService.Models;

namespace AttachmentService.Interfaces
{
    public interface IAttachmentFactoryService
    {
        Attachment CreateAttachment(
            Guid attachmentGuid,
            string fileName,
            EntityMapping entityMapping,
            Guid buildingGuid,
            Guid userUuid,
            string fileContentType
        );
    }
}