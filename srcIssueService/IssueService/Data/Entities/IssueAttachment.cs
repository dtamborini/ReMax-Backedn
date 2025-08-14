using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace IssueService.Data.Entities;

public class IssueAttachment : BaseEntity
{
    // FK a Issue
    [Required]
    public Guid IssueId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    [Required]
    public Guid AttachmentId { get; set; }
}