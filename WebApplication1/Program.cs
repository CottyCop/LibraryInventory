using Infra.Data;                    // donde vive AppDbContext
using Microsoft.EntityFrameworkCore; // AddDbContext / UseSqlServer



var builder = WebApplication.CreateBuilder(args);
//  Lee la connection string
var connectionString = builder.Configuration.GetConnectionString("Default");

var allowedOrigins = new[] { "http://127.0.0.1:5500", "http://localhost:5500" };

builder.Services.AddCors(o =>
{
    o.AddPolicy("dev", p => p
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// Registra tu DbContext en el contenedor DI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("dev");

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
