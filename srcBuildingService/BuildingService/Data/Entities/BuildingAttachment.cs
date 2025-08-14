using System.ComponentModel.DataAnnotations;
using BuildingService.Data.Enums;
using RemaxManagement.Shared.Data.Entities;

namespace BuildingService.Data.Entities;

public class BuildingAttachment : BaseEntity
{
    [Required]
    public AttachmentType Type { get; set; }
    
    [Required]
    public DateTime LastModify { get; set; }
    
    [Required]
    public long Size { get; set; }
    
    // FK a se stesso per struttura gerarchica (nullable per root items)
    public Guid? FatherId { get; set; }
    
    // FK a Building
    [Required]
    public Guid BuildingId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    public Guid? AttachmentId { get; set; }
}