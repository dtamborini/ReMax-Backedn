using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;
using RfqService.Data.Enums;

namespace RfqService.Data.Entities;

public class RFQ : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Object { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime ExpireDateSupplierResponse { get; set; }

    [Required]
    public DateTime ExpireDateRequestForWork { get; set; }

    [Required]
    public RfqPriority Priority { get; set; } = RfqPriority.Medium;

    public bool ExtraordinaryIntervention { get; set; } = false;

    public bool RecommendedRetailPrice { get; set; } = false;

    public bool ReducedVatRate { get; set; } = false;

    [Required]
    public Guid BuildingId { get; set; }

    [Required]
    public Guid WorkSheetId { get; set; }

    [Required]
    public Guid WorkOrderId { get; set; }
}