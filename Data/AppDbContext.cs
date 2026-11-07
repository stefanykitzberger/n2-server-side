using Microsoft.EntityFrameworkCore;
using ApiClinica.Models;

namespace ApiClinica.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Medico> Medicos { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Garante no banco que não existam dois usuários com o mesmo email.
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}