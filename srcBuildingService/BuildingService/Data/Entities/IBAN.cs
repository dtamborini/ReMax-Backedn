using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using RemaxManagement.Shared.Data.Entities;

namespace BuildingService.Data.Entities;

public class IBAN : BaseEntity
{
    [Required]
    [MaxLength(34)] // Maximum IBAN length is 34 characters
    [RegularExpression(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]+$", ErrorMessage = "Invalid IBAN format")]
    public string Code { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    // Foreign key to Building
    [Required]
    public Guid BuildingId { get; set; }
    
    [ForeignKey(nameof(BuildingId))]
    public virtual Building Building { get; set; } = null!;
}