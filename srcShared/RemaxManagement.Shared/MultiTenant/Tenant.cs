using Finbuckle.MultiTenant;

namespace RemaxManagement.Shared.MultiTenant;

public class Tenant : ITenantInfo
{
    // ITenantInfo richiede string?, quindi convertiamo GUID in string
    public string? Id { get; set; } = Guid.Empty.ToString();
    
    // Proprietà GUID effettiva per il database
    public Guid TenantId { get; set; } = Guid.Empty;
    
    public string? Identifier { get; set; } = string.Empty;      // "condominio-rossi"
    public string? Name { get; set; } = string.Empty;            // "Condominio Via Rossi 15"
    public string? ConnectionString { get; set; } = string.Empty;
    
    // Proprietà aggiuntive per ReMax
    public string AdminEmail { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Relazione con domini (1:N)
    public List<TenantDomain> Domains { get; set; } = new();
    
    // Schema PostgreSQL calcolato
    public string SchemaName => $"tenant_{Identifier}";
}