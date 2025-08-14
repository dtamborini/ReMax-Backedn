using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;
using WorkOrderService.Data.Enums;

namespace WorkOrderService.Data.Entities;

public class Intervention : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime SchedulingDate { get; set; }

    [Required]
    public InterventionOutcome Outcome { get; set; } = InterventionOutcome.Pending;

    public string? FinalDescription { get; set; }

    [Required]
    public Guid WorkOrderId { get; set; }
    public virtual WorkOrder WorkOrder { get; set; } = null!;
}