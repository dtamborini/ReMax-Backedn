using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace RfqService.Data.Entities;

public class RfqSupplier : BaseEntity
{
    [Required]
    public Guid SupplierId { get; set; }

    [Required]
    public Guid RfqId { get; set; }
    public virtual RFQ RFQ { get; set; } = null!;
}