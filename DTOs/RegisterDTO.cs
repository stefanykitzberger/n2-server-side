using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class RegisterDTO
{
    public required string Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; set; }

    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public required string Senha { get; set; }
}
