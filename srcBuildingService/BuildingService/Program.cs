using Microsoft.AspNetCore.Mvc;
using RemaxManagement.Shared.Extensions;
using RemaxManagement.Shared.Data.Extensions;
using BuildingService.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Usa la configurazione JWT shared
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configura database PostgreSQL con schema buildings
builder.Services.AddPostgreSqlDatabase<BuildingDbContext>(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Placeholder per API - implementeremo con controller successivamente
app.MapGet("/health", () => "Building Service is running")
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();

