using ApiClinica.Data;
using ApiClinica.Interfaces;
using ApiClinica.Mappers;
using ApiClinica.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Adiciona o banco (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=clinica.db"));

// Injeção de dependências: Mappers e Services
builder.Services.AddScoped<IPacienteMapper, PacienteMapper>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicoMapper, MedicoMapper>();
builder.Services.AddScoped<IMedicoService, MedicoService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();