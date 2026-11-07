using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class LoginDTO
{
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; set; }

    public required string Senha { get; set; }
}
