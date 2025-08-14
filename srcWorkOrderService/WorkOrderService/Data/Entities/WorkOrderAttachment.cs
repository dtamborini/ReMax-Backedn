using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace WorkOrderService.Data.Entities;

public class WorkOrderAttachment : BaseEntity
{
    // FK a WorkOrder
    [Required]
    public Guid WorkOrderId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    [Required]
    public Guid AttachmentId { get; set; }
}