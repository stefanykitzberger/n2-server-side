using System.ComponentModel.DataAnnotations;

namespace ApiClinica.Models;

public class Medico
{
    public int Id { get; set; }

    public required string Nome { get; set; }
    
    [EmailAddress(ErrorMessage = "Email inválido")]
    public required string Email { get; set; }
    
    [RegularExpression(@"^\(\d{2}\) \d{4,5}-\d{4}$", ErrorMessage = "Telefone inválido. Formato esperado: (47) 98888-7777.")]
    public required string Telefone { get; set; }

    public required string CRM { get; set; }
}