using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;
using WorkSheetService.Data.Enums;

namespace WorkSheetService.Data.Entities;

public class WorkSheets : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public WorkSheetPriority Priority { get; set; } = WorkSheetPriority.Medium;

    [Required]
    public DateTime ExpireDate { get; set; }

    [Required]
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid BuildingId { get; set; }
}