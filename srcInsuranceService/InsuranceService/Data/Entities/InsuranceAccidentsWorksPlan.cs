using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace InsuranceService.Data.Entities;

public class InsuranceAccidentsWorksPlan : BaseEntity
{
    // FK a InsuranceAccidents
    [Required]
    public Guid InsuranceAccidentId { get; set; }
    
    // FK to WorkPlans (cross-microservice reference stored as Guid)
    [Required]
    public Guid WorkPlanId { get; set; }
}