using System.Text;
using ApiClinica.Data;
using ApiClinica.Interfaces;
using ApiClinica.Mappers;
using ApiClinica.Models;
using ApiClinica.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddScoped<IConsultaMapper, ConsultaMapper>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();

// Injeção de dependências: Autenticação
builder.Services.AddScoped<IUsuarioMapper, UsuarioMapper>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Autenticação JWT: valida assinatura, emissor, público e expiração do token enviado no header Authorization.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };

        // Respostas 401 e 403 com mensagem em JSON (por padrão vêm com corpo vazio).
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { mensagem = "Token ausente, inválido ou expirado." });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { mensagem = "Acesso negado. Apenas usuários admin podem realizar esta operação." });
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    await authService.CriarAdminPadraoAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
