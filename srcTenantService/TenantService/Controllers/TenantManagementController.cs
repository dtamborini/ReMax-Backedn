using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemaxManagement.Shared.MultiTenant;
using RemaxManagement.Shared.Extensions;
using TenantService.Data.Context;
using TenantService.DTOs;
using TenantService.Services;

namespace TenantService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantManagementController : ControllerBase
{
    private readonly TenantManagementDbContext _context;
    private readonly ILogger<TenantManagementController> _logger;
    private readonly ITenantSchemaService _tenantSchemaService;

    public TenantManagementController(
        TenantManagementDbContext context,
        ILogger<TenantManagementController> logger,
        ITenantSchemaService tenantSchemaService)
    {
        _context = context;
        _logger = logger;
        _tenantSchemaService = tenantSchemaService;
    }

    /// <summary>
    /// Ottiene tutti i tenant
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
    {
        var tenants = await _context.Tenants
            .Include(t => t.Domains)
            .ToListAsync();
        
        return Ok(tenants);
    }

    /// <summary>
    /// Ottiene un tenant per ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Tenant>> GetTenant(Guid id)
    {
        var tenant = await _context.Tenants
            .Include(t => t.Domains)
            .FirstOrDefaultAsync(t => t.TenantId == id);

        if (tenant == null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    /// <summary>
    /// Crea un nuovo tenant
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<Tenant>> CreateTenant(CreateTenantRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verifica se esiste già un tenant con lo stesso identifier
        var existingTenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Identifier == request.Identifier);

        if (existingTenant != null)
        {
            return Conflict("Un tenant con questo identificatore esiste già");
        }

        // Genera ID univoco
        var tenantId = Guid.NewGuid();

        // Costruisce connection string per il tenant
        var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var baseConnectionString = configuration.GetConnectionString("DefaultConnection");
        var schemaName = $"tenant_{request.Identifier}";
        var tenantConnectionString = MultiTenantExtensions.BuildConnectionStringForSchema(baseConnectionString!, schemaName);

        var tenant = new Tenant
        {
            TenantId = tenantId,
            Id = tenantId.ToString(), // Per ITenantInfo
            Identifier = request.Identifier,
            Name = request.Name,
            ConnectionString = tenantConnectionString,
            AdminEmail = request.AdminEmail,
            Address = request.Address,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Aggiungi domini se presenti
        if (request.Domains?.Any() == true)
        {
            tenant.Domains = request.Domains.Select(domain => new TenantDomain
            {
                Domain = domain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }).ToList();
        }

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created new tenant: {TenantId} ({TenantName})", tenant.TenantId, tenant.Name);

        // Crea lo schema tenant e applica le migrazioni
        try
        {
            await _tenantSchemaService.CreateTenantSchemaAsync(tenant);
            await _tenantSchemaService.ApplyMigrationsToTenantSchemaAsync(tenant);
            _logger.LogInformation("Successfully set up tenant schema for {TenantId}", tenant.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set up tenant schema for {TenantId}", tenant.TenantId);
            // Potresti voler fare rollback del tenant qui se la creazione dello schema fallisce
        }

        return CreatedAtAction(nameof(GetTenant), new { id = tenant.TenantId }, tenant);
    }

    /// <summary>
    /// Aggiorna un tenant esistente
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateTenant(Guid id, UpdateTenantRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tenant = await _context.Tenants
            .Include(t => t.Domains)
            .FirstOrDefaultAsync(t => t.TenantId == id);

        if (tenant == null)
        {
            return NotFound();
        }

        // Aggiorna proprietà base
        tenant.Name = request.Name;
        tenant.AdminEmail = request.AdminEmail;
        tenant.Address = request.Address;
        tenant.IsActive = request.IsActive;

        // Aggiorna domini se specificati
        if (request.Domains != null)
        {
            // Rimuovi domini esistenti
            _context.TenantDomains.RemoveRange(tenant.Domains);
            
            // Aggiungi nuovi domini
            tenant.Domains = request.Domains.Select(domain => new TenantDomain
            {
                Domain = domain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }).ToList();
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated tenant: {TenantId} ({TenantName})", tenant.TenantId, tenant.Name);

        return NoContent();
    }

    /// <summary>
    /// Elimina un tenant
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteTenant(Guid id)
    {
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.TenantId == id);

        if (tenant == null)
        {
            return NotFound();
        }

        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted tenant: {TenantId} ({TenantName})", tenant.TenantId, tenant.Name);

        return NoContent();
    }

    /// <summary>
    /// Ottiene un tenant per dominio
    /// </summary>
    [HttpGet("by-domain/{domain}")]
    [Authorize]
    public async Task<ActionResult<Tenant>> GetTenantByDomain(string domain)
    {
        var tenant = await _context.Tenants
            .Include(t => t.Domains)
            .Where(t => t.IsActive)
            .FirstOrDefaultAsync(t => t.Domains.Any(d => d.Domain == domain && d.IsActive));

        if (tenant == null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }
}