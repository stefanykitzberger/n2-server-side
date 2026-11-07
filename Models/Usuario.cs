namespace ApiClinica.Models;

public class Usuario
{
    public int Id { get; set; }

    public required string Nome { get; set; }

    public required string Email { get; set; }

    // Nunca armazenamos a senha em texto puro, apenas o hash gerado pelo PasswordHasher.
    public string SenhaHash { get; set; } = default!;

    // "Comum" ou "Admin" (ver Roles.cs).
    public required string Role { get; set; }
}
