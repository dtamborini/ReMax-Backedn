using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace InsuranceService.Data.Entities;

public class InsuranceDeductible : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Guarantee { get; set; } = string.Empty;
    
    [Required]
    public double Amount { get; set; }
    
    // FK a Insurance
    [Required]
    public Guid InsuranceId { get; set; }
}