using Microsoft.AspNetCore.Mvc;
using RemaxManagement.Shared.Extensions;
using RemaxManagement.Shared.Data.Extensions;
using WorkSheetService.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddPostgreSqlDatabase<WorkSheetDbContext>(builder.Configuration);

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

app.MapGet("/health", () => "WorkSheet Service is running")
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();