using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RemaxManagement.Shared.Extensions;
using RemaxManagement.Shared.Data.Extensions;
using BuildingService.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();

// Add CORS for Swagger UI
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Building Service API",
        Version = "v1",
        Description = "API per la gestione degli edifici e delle relative informazioni (IBANs, attachments)"
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
    
    // Aggiungi supporto per il header Tenant
    options.AddSecurityDefinition("Tenant", new OpenApiSecurityScheme
    {
        Description = "Tenant header per identificare il tenant. Esempio: \"test\"",
        Name = "Tenant",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Tenant"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add HttpClient for AttachmentService
builder.Services.AddHttpClient("AttachmentService", client =>
{
    var attachmentServiceUrl = builder.Configuration["Services:AttachmentService:Url"] ?? "http://localhost:5080";
    client.BaseAddress = new Uri(attachmentServiceUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Usa la configurazione JWT shared
builder.Services.AddJwtAuthentication(builder.Configuration);

// Aggiungi logging per debug JWT
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Configurazione multi-tenancy 
builder.Services.AddRemaxMultiTenancy(builder.Configuration);

// Configura database PostgreSQL multi-tenant
builder.Services.AddMultiTenantDatabase<BuildingDbContext>(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

// Non applichiamo migrazioni automatiche - gestite dal TenantService
// app.ApplyMigrations<BuildingDbContext>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Building Service API v1");
        options.RoutePrefix = string.Empty; // Swagger UI alla root
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

// Multi-tenant middleware DEVE essere dopo Authentication
app.UseMultiTenant();

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => "Building Service is running")
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();

