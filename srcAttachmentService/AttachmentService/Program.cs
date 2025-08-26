using Microsoft.AspNetCore.Mvc;
using RemaxManagement.Shared.Extensions;
using RemaxManagement.Shared.Data.Extensions;
using AttachmentService.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Usa la configurazione JWT shared
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configurazione multi-tenancy 
builder.Services.AddRemaxMultiTenancy(builder.Configuration);

// Configura database PostgreSQL multi-tenant
builder.Services.AddMultiTenantDatabase<AttachmentDbContext>(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

// Non applichiamo migrazioni automatiche - gestite dal TenantService
// app.ApplyMigrations<AttachmentDbContext>();

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

app.MapGet("/health", () => "Attachment Service is running")
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();