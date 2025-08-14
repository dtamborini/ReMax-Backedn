using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace RfqService.Data.Entities;

public class NegotiationAttachment : BaseEntity
{
    // FK a Negotiation
    [Required]
    public Guid NegotiationId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    [Required]
    public Guid AttachmentId { get; set; }
}