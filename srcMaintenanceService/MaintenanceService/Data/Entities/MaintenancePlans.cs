using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace MaintenanceService.Data.Entities;

public class MaintenancePlans : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Object { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public int Frequency { get; set; }
    
    // FK a Supplier (cross-microservice reference stored as Guid)
    [Required]
    public Guid SupplierId { get; set; }
    
    // FK a PremisesBuilding (cross-microservice reference stored as Guid)
    [Required]
    public Guid PremisesBuildingId { get; set; }
}