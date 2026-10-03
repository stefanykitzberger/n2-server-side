using System.ComponentModel.DataAnnotations;

namespace ApiClinica.DTOs;

public class PacienteUpdateDTO
{
    public string? Nome { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }

    [RegularExpression(@"^\(\d{2}\) \d{4,5}-\d{4}$", ErrorMessage = "Telefone inválido. Formato esperado: (47) 98888-7777.")]
    public string? Telefone { get; set; }

    public DateOnly? DataNasc { get; set; }

    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter 11 dígitos numéricos.")]
    public string? Cpf { get; set; }
}
