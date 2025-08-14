using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace RfqService.Data.Entities;

public class Quotes : BaseEntity
{
    public bool Accepted { get; set; } = false;

    [Required]
    public double InitialPrice { get; set; }

    [Required]
    public DateTime InitialResolutionDate { get; set; }

    public double? FinalPrice { get; set; }

    public DateTime? FinalResolutionDate { get; set; }

    public string? Note { get; set; }

    [Required]
    public Guid RfqId { get; set; }
    public virtual RFQ RFQ { get; set; } = null!;

    [Required]
    public Guid SupplierId { get; set; }
}