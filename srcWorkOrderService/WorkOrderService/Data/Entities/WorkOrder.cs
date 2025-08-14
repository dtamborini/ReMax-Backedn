using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;
using WorkOrderService.Data.Enums;

namespace WorkOrderService.Data.Entities;

public class WorkOrder : BaseEntity
{
    [Required]
    public WorkOrderType Type { get; set; }

    public bool Accepted { get; set; } = false;

    [Required]
    [MaxLength(200)]
    public string Object { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime ExpireDate { get; set; }

    [Required]
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;

    public bool Extraordinary { get; set; } = false;

    public bool RecommendedRetailPrice { get; set; } = false;

    public bool ReducedVatRate { get; set; } = false;

    [Required]
    public Guid BuildingId { get; set; }

    [Required]
    public Guid WorkSheetId { get; set; }

    [Required]
    public Guid RfqId { get; set; }
}