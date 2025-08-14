using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace WorkOrderService.Data.Entities;

public class InterventionAttachment : BaseEntity
{
    // FK a Intervention
    [Required]
    public Guid InterventionId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    [Required]
    public Guid AttachmentId { get; set; }
}