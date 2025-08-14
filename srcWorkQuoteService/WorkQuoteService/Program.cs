using Microsoft.AspNetCore.Mvc;
using RemaxManagement.Shared.Extensions;
using RemaxManagement.Shared.Data.Extensions;
using WorkQuoteService.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Usa la configurazione JWT shared
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configura database PostgreSQL con schema work_quotes
builder.Services.AddPostgreSqlDatabase<WorkQuoteDbContext>(builder.Configuration);

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

app.MapGet("/health", () => "WorkQuote Service is running")
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();