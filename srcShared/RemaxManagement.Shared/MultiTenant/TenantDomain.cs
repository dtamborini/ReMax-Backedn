using System.Text.Json.Serialization;

namespace RemaxManagement.Shared.MultiTenant;

public class TenantDomain
{
    public int Id { get; set; }
    public Guid TenantId { get; set; }
    public string Domain { get; set; } = string.Empty;          // "condominio-rossi.remax.com"
    public bool IsPrimary { get; set; } = false;                // Dominio principale
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    [JsonIgnore]
    public Tenant Tenant { get; set; } = null!;
}