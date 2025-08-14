using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RemaxManagement.Shared.Data.Entities;
using BuildingService.Data.Enums;

namespace BuildingService.Data.Entities;

public class PremisesBuilding : BaseEntity
{
    // Self-referencing foreign key for hierarchical structure
    public Guid? FatherId { get; set; }
    
    [ForeignKey(nameof(FatherId))]
    public virtual PremisesBuilding? Father { get; set; }
    
    // Type of premises
    [Required]
    public PremisesType Type { get; set; }
    
    // Note field
    [MaxLength(1000)]
    public string? Note { get; set; }
    
    // Foreign key to Building
    [Required]
    public Guid BuildingId { get; set; }
    
    [ForeignKey(nameof(BuildingId))]
    public virtual Building Building { get; set; } = null!;
    
    // Type tags list stored as JSON array of PremisesType enums
    [Column(TypeName = "jsonb")]
    public List<PremisesType> TypeTags { get; set; } = new List<PremisesType>();
    
    // Navigation property for children
    public virtual ICollection<PremisesBuilding> Children { get; set; } = new List<PremisesBuilding>();
}