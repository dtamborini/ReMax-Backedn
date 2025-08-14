using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace CheckListService.Data.Entities;

public class CheckListGroup : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public bool Complete { get; set; } = false;
    
    // FK to WorkOrder (cross-microservice reference stored as Guid)
    [Required]
    public Guid WorkOrderId { get; set; }
}