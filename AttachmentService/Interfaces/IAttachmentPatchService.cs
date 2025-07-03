using AttachmentService.Models;

namespace AttachmentService.Interfaces
{
    public interface IAttachmentPatchService
    {
        Task ApplyAttachmentPatch(Attachment existingAttachment, Attachment patchDto, EntityParticipation userParticipation);
    }
}