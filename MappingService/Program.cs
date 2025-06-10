// MappingService/Program.cs
using Microsoft.EntityFrameworkCore;
using MappingService.Data; // Assicurati che il namespace sia corretto

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Aggiungi la configurazione del DbContext per PostgreSQL
builder.Services.AddDbContext<MappingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MappingDbConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aggiungi la migrazione automatica e il seeding (se necessario)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MappingDbContext>();
    dbContext.Database.Migrate();
}

app.UseAuthorization();

app.MapControllers();

app.Run();