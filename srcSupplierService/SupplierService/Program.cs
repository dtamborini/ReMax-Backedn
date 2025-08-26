using RemaxManagement.Shared.Extensions;
using RemaxManagement.Shared.Data.Extensions;
using SupplierService.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Usa la configurazione JWT shared
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configurazione multi-tenancy 
builder.Services.AddRemaxMultiTenancy(builder.Configuration);

// Configura database PostgreSQL multi-tenant
builder.Services.AddMultiTenantDatabase<SupplierDbContext>(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

// Non applichiamo migrazioni automatiche - gestite dal TenantService
// app.ApplyMigrations<SupplierDbContext>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();

// Multi-tenant middleware DEVE essere dopo Authentication
app.UseMultiTenant();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => "Supplier Service is running")
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();