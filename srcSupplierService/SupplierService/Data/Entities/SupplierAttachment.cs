using System.ComponentModel.DataAnnotations;
using SupplierService.Data.Enums;
using RemaxManagement.Shared.Data.Entities;

namespace SupplierService.Data.Entities;

public class SupplierAttachment : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public AttachmentState State { get; set; }
    
    [Required]
    public DateTime ExpireDate { get; set; }
    
    // FK to Building (cross-microservice reference stored as Guid)
    [Required]
    public Guid BuildingId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    [Required]
    public Guid AttachmentId { get; set; }
}