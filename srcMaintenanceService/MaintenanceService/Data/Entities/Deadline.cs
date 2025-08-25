using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace MaintenanceService.Data.Entities;

public class Deadline : BaseEntity
{
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Object { get; set; } = string.Empty;
    
    // FK a MaintenancePlans
    [Required]
    public Guid MaintenancePlanId { get; set; }
    
    //ToDO: mettere buildingId per semplicità
}