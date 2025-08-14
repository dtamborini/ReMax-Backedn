using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace BuildingService.Data.Entities;

public class PolicyAttachment : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    // FK a Building
    [Required]
    public Guid BuildingId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    [Required]
    public Guid AttachmentId { get; set; }
}