namespace ApiClinica.DTOs;

public class UsuarioReadDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Role { get; set; } = default!;
}
