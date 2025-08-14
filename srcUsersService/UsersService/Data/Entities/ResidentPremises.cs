using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RemaxManagement.Shared.Data.Entities;

namespace UsersService.Data.Entities;

public class ResidentPremises : BaseEntity
{
    // Percentage of ownership/usage (nullable)
    public double? Percentage { get; set; }
    
    // Foreign key to Building (from BuildingService)
    [Required]
    public Guid BuildingId { get; set; }
    
    // Foreign key to PremisesBuilding (from BuildingService)
    [Required]
    public Guid PremisesBuildingId { get; set; }
    
    // Foreign key to Resident
    [Required]
    public Guid ResidentId { get; set; }
    
    [ForeignKey(nameof(ResidentId))]
    public virtual Resident Resident { get; set; } = null!;
    
    // Note: Building and PremisesBuilding navigation properties are not included
    // because they belong to another microservice (BuildingService).
    // We only store the foreign key references.
}