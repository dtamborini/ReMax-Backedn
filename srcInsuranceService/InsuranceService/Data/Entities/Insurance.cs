using System.ComponentModel.DataAnnotations;
using InsuranceService.Data.Enums;
using RemaxManagement.Shared.Data.Entities;

namespace InsuranceService.Data.Entities;

public class Insurance : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string PolicyNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Company { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Agent { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? AgentPhoneNumber { get; set; }
    
    [Required]
    public InsuranceType Type { get; set; }
    
    [Required]
    public DateTime AgreementDate { get; set; }
    
    [Required]
    public DateTime ExpireDate { get; set; }
    
    [Required]
    public double InsurancePremium { get; set; }
    
    [Required]
    public double Fractionation { get; set; }
    
    public string? Previsions { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    public Guid? PolicyDocumentId { get; set; }
    
    // FK to Attachment (cross-microservice reference stored as Guid)
    public Guid? ReceiptDocumentId { get; set; }
}