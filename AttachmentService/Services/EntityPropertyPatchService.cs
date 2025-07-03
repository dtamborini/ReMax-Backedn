using AttachmentService.Interfaces;
using AttachmentService.Models;
using AttachmentService.Enums;

namespace AttachmentService.Services
{
    public class EntityPropertyPatchService : IEntityPropertyPatchService
    {
        public void ApplyEntityPropertyPatch(
            EntityProperty existingProperty,
            EntityProperty patchProperty,
            EntityParticipation userParticipation)
        {
            if (existingProperty == null || patchProperty == null)
            {
                return;
            }

            if (patchProperty.Value != null)
            {
                existingProperty.Value = patchProperty.Value;
            }

            if (patchProperty.Dates != null)
            {
                existingProperty.Dates = patchProperty.Dates;
            }

            if (patchProperty.Properties != null && patchProperty.Properties.Any())
            {
                if (existingProperty.Properties == null)
                {
                    existingProperty.Properties = new List<EntityProperty>();
                }

                foreach (var subPatchProperty in patchProperty.Properties)
                {

                    var existingSubProperty = existingProperty.Properties.FirstOrDefault(p => p.Guid == subPatchProperty.Guid);

                    if (existingSubProperty != null)
                    {
                        ApplyEntityPropertyPatch(existingSubProperty, subPatchProperty, userParticipation);
                    }
                    else
                    {
                        existingProperty.Properties.Add(subPatchProperty);
                    }
                }
            }
            
            else if (patchProperty.Properties != null && !patchProperty.Properties.Any())
            {
                existingProperty.Properties = new List<EntityProperty>();
            }

            if (existingProperty.Dates != null)
            {
                existingProperty.Dates.Add(new EntityDate
                {
                    DateType = DateType.Updated,
                    User = userParticipation
                });
            }
            else
            {
                existingProperty.Dates = new List<EntityDate>
                {
                    new EntityDate
                    {
                        DateType = DateType.Updated,
                        User = userParticipation
                    }
                };
            }
        }

        public List<EntityProperty> ApplyEntityPropertyPatchToList(
            List<EntityProperty> existingProperties,
            List<EntityProperty> patchProperties,
            EntityParticipation userParticipation)
        {
            if (existingProperties == null)
            {
                existingProperties = new List<EntityProperty>();
                foreach (var newProperty in patchProperties)
                {
                    existingProperties.Add(newProperty);

                    newProperty.Dates ??= new List<EntityDate>();
                    newProperty.Dates.Add(new EntityDate
                    {
                        DateType = DateType.Created,
                        User = userParticipation
                    });
                }
                return existingProperties;
            }

            if (patchProperties == null || !patchProperties.Any())
            {
                return existingProperties;
            }

            foreach (var patchProperty in patchProperties)
            {
                var existingProperty = existingProperties.FirstOrDefault(p => p.Guid == patchProperty.Guid);
                if (existingProperty != null)
                {
                    ApplyEntityPropertyPatch(existingProperty, patchProperty, userParticipation);
                }
                else
                {
                    existingProperties.Add(patchProperty);
                    patchProperty.Dates ??= new List<EntityDate>();
                    patchProperty.Dates.Add(new EntityDate
                    {
                        DateType = DateType.Created,
                        User = userParticipation
                    });
                }
            }

            return existingProperties;
        }
    }
}