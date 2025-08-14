using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace BuildingService.Data.Entities;

public class Building : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [MaxLength(100)]
    public string? Pec { get; set; }
    
    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }
    
    [Required]
    [MaxLength(16)]
    public string FiscalCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(11)]
    public string VatCode { get; set; } = string.Empty;
}