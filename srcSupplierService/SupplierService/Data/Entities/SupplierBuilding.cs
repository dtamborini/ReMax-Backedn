using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace SupplierService.Data.Entities;

public class SupplierBuilding : BaseEntity
{
    public bool Favorite { get; set; } = false;

    [Required]
    public Guid BuildingId { get; set; }

    [Required] 
    public Guid SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; } = null!;
}