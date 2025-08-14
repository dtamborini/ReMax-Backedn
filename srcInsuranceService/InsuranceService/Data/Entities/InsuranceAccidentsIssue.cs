using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace InsuranceService.Data.Entities;

public class InsuranceAccidentsIssue : BaseEntity
{
    // FK to Issue (cross-microservice reference stored as Guid)
    [Required]
    public Guid IssueId { get; set; }
    
    // FK a InsuranceAccidents
    [Required]
    public Guid InsuranceAccidentId { get; set; }
}