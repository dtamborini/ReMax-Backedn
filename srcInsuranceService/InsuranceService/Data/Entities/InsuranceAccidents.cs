using System.ComponentModel.DataAnnotations;
using InsuranceService.Data.Enums;
using RemaxManagement.Shared.Data.Entities;

namespace InsuranceService.Data.Entities;

public class InsuranceAccidents : BaseEntity
{
    [Required]
    public PracticalStatus PracticalStatus { get; set; }
    
    public string? Description { get; set; }
    
    [Required]
    public double EstimatedDamage { get; set; }
    
    [Required]
    public double AmountPaid { get; set; }
    
    // FK to Building (cross-microservice reference stored as Guid)
    [Required]
    public Guid BuildingId { get; set; }
    
    // FK a Insurance
    [Required]
    public Guid InsuranceId { get; set; }
}