namespace ApiClinica.DTOs;

public class ConsultaCreateDTO
{
    public int PacienteId { get; set; }

    public int MedicoId { get; set; }

    public required DateTime DataHora { get; set; }
}
