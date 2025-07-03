using AttachmentService.Models;
using System.Collections.Generic;

namespace AttachmentService.Interfaces
{
    public interface IEntityPropertyPatchService
    {
        void ApplyEntityPropertyPatch(
            EntityProperty existingProperty,
            EntityProperty patchProperty, 
            EntityParticipation userParticipation);
        List<EntityProperty> ApplyEntityPropertyPatchToList(
            List<EntityProperty> existingProperties,
            List<EntityProperty> patchProperties,
            EntityParticipation userParticipation);
    }
}