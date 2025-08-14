using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemaxManagement.Shared.Data.Entities;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Custom data for extensibility
    [Column(TypeName = "jsonb")]
    public string? CustomData { get; set; }
    
    // Versioning for optimistic concurrency
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}