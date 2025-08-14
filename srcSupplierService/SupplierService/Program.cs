using RemaxManagement.Shared.Extensions;
using RemaxManagement.Shared.Data.Extensions;
using SupplierService.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Use shared JWT configuration
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure PostgreSQL database with suppliers schema
builder.Services.AddPostgreSqlDatabase<SupplierDbContext>(builder.Configuration);

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

app.MapGet("/health", () => "Supplier Service is running")
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();