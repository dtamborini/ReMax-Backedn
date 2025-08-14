using System.ComponentModel.DataAnnotations;
using RemaxManagement.Shared.Data.Entities;

namespace RfqService.Data.Entities;

public class Negotiation : BaseEntity
{
    public bool Accepted { get; set; } = false;

    [Required]
    public double Amount { get; set; }

    [Required]
    public DateTime ResolutionDate { get; set; }

    public string? Note { get; set; }

    [Required]
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid QuotesId { get; set; }
    public virtual Quotes Quotes { get; set; } = null!;
}