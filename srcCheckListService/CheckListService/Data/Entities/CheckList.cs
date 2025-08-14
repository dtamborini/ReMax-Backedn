using System.ComponentModel.DataAnnotations;
using CheckListService.Data.Enums;
using RemaxManagement.Shared.Data.Entities;

namespace CheckListService.Data.Entities;

public class CheckList : BaseEntity
{
    [Required]
    public CheckListType Type { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public bool RequiredDownload { get; set; } = false;
    
    public bool ReadReceipt { get; set; } = false;
    
    public bool Required { get; set; } = false;
    
    // FK to Attachment (cross-microservice reference stored as Guid, nullable)
    public Guid? AttachmentId { get; set; }
    
    // FK to BuildingAttachment (cross-microservice reference stored as Guid, nullable)
    public Guid? BuildingAttachmentId { get; set; }
    
    // FK a CheckListGroup
    [Required]
    public Guid CheckListGroupId { get; set; }
}