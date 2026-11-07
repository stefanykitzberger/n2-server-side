namespace ApiClinica.DTOs;

// Resposta do login: o token JWT e os dados básicos do usuário autenticado.
public class TokenDTO
{
    public string Token { get; set; } = default!;

    public DateTime ExpiraEm { get; set; }

    public UsuarioReadDTO Usuario { get; set; } = default!;
}
