using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace IssueService.Data.Entities;

public class IssueWorkSheet : BaseEntity 
{
    // FK a Issue
    [Required]
    public Guid IssueId { get; set; }
    
    // FK to WorkSheets (cross-microservice reference stored as Guid)
    [Required]
    public Guid WorkSheetId { get; set; }
    
}