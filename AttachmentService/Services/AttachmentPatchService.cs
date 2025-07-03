// Services/AttachmentPatchService.cs
using AttachmentService.Interfaces;
using AttachmentService.Models;

namespace AttachmentService.Services
{
    public class AttachmentPatchService : IAttachmentPatchService
    {
        private readonly IEntityPropertyPatchService _entityPropertyPatchService;

        public AttachmentPatchService(IEntityPropertyPatchService entityPropertyPatchService)
        {
            _entityPropertyPatchService = entityPropertyPatchService;
        }

        public async Task ApplyAttachmentPatch(
            Attachment existingAttachment,
            Attachment patchDto,
            EntityParticipation userParticipation)
        {
            if (existingAttachment == null || patchDto == null)
            {
                return;
            }

            existingAttachment.DeserializeComplexData();

            if (patchDto.Name != null)
            {
                existingAttachment.Name = patchDto.Name;
            }

            if (patchDto.UniqueIdentifiers != null)
            {
                existingAttachment.UniqueIdentifiers = patchDto.UniqueIdentifiers;
            }

            if (patchDto.States != null)
            {
                existingAttachment.States = patchDto.States;
            }

            if (patchDto.Participations != null)
            {
                existingAttachment.Participations = patchDto.Participations;
            }

            if (patchDto.Attachments != null)
            {
                existingAttachment.Attachments = patchDto.Attachments;
            }

            if (patchDto.Properties != null)
            {
                existingAttachment.Properties ??= new List<EntityProperty>();
                _entityPropertyPatchService.ApplyEntityPropertyPatchToList(
                    existingAttachment.Properties,
                    patchDto.Properties,
                    userParticipation);
            }

            existingAttachment.SerializeComplexData();
        }
    }
}