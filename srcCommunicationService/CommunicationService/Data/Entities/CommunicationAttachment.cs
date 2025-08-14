using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace CommunicationService.Data.Entities;

public class CommunicationAttachment : BaseEntity
{
    // FK a Communication
    [Required]
    public Guid CommunicationId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    [Required]
    public Guid AttachmentId { get; set; }
}