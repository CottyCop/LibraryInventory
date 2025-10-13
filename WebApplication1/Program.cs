using Infra.Data;                    // donde vive AppDbContext
using Microsoft.EntityFrameworkCore; // AddDbContext / UseSqlServer



var builder = WebApplication.CreateBuilder(args);
//  Lee la connection string
var connectionString = builder.Configuration.GetConnectionString("Default");

// Registra tu DbContext en el contenedor DI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
