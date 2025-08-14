using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;
using IssueService.Data.Enums;

namespace IssueService.Data.Entities;

public class Issue : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Object { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime Data { get; set; } = DateTime.UtcNow;

    [Required]
    public IssueState State { get; set; } = IssueState.Confirmed;

    public string? Motivation { get; set; }

    public bool IsPublic { get; set; } = true;

    [Required]
    public Guid BuildingId { get; set; }
}