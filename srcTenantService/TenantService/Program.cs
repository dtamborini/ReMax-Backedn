using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RemaxManagement.Shared.Extensions;
using RemaxManagement.Shared.Data.Extensions;
using TenantService.Data.Context;
using TenantService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tenant Service API",
        Version = "v1",
        Description = "API per la gestione dei tenant multi-tenancy"
    });
    
    // Configurazione JWT per Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando il Bearer scheme. Esempio: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Usa la configurazione JWT shared
builder.Services.AddJwtAuthentication(builder.Configuration);

// Database per gestione tenant (schema pubblico)
builder.Services.AddDbContext<TenantManagementDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Configurazione multi-tenancy per risoluzione tenant
builder.Services.AddRemaxMultiTenancy(builder.Configuration);

// Registra il servizio per gestione schema tenant
builder.Services.AddScoped<ITenantSchemaService, TenantSchemaService>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Apply pending migrations for tenant management
app.ApplyMigrations<TenantManagementDbContext>();

// Seed initial Super Admin
await SeedDataAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tenant Service API v1");
        options.RoutePrefix = string.Empty; // Swagger UI alla root
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Multi-tenant middleware
app.UseMultiTenant();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => "Tenant Service is running")
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();

// Metodo helper per il seeding
async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TenantManagementDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        await TenantService.Data.Seeding.SuperAdminSeeder.SeedSuperAdminAsync(context);
        logger.LogInformation("Data seeding completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during data seeding");
    }
}