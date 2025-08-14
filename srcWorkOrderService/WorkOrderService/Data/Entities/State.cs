using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;
using WorkOrderService.Data.Enums;

namespace WorkOrderService.Data.Entities;

public class State : BaseEntity
{
    [Required]
    public StateType Type { get; set; }

    [Required]
    public DateTime Data { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid InterventionId { get; set; }
    public virtual Intervention Intervention { get; set; } = null!;
}