namespace ApiClinica.Models;

public class Paciente
{
    public int Id { get; set; }

    public required string Nome { get; set; }
    
    public required string Email { get; set; }
    
    public required string Telefone { get; set; }
    
    public required DateOnly DataNasc { get; set; }
    
    public required string Cpf { get; set; }
}