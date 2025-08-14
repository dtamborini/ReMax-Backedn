using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace RfqService.Data.Entities;

public class QuoteAttachment : BaseEntity
{
    // FK a Quotes
    [Required]
    public Guid QuoteId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    [Required]
    public Guid AttachmentId { get; set; }
}