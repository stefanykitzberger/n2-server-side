using ApiClinica.Models;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class ConsultaMapper : IConsultaMapper
{
    public Consulta ToModel(ConsultaCreateDTO dto)
    {
        return new Consulta()
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            DataHora = dto.DataHora
        };
    }

    public void UpdateModel(Consulta consulta, ConsultaUpdateDTO dto)
    {
        if (dto.PacienteId != null) consulta.PacienteId = dto.PacienteId.Value;
        if (dto.MedicoId != null) consulta.MedicoId = dto.MedicoId.Value;
        if (dto.DataHora != null) consulta.DataHora = dto.DataHora.Value;
    }

    public ConsultaReadDTO ToDTO(Consulta consulta)
    {
        return new ConsultaReadDTO()
        {
            Id = consulta.Id,
            PacienteId = consulta.PacienteId,
            MedicoId = consulta.MedicoId,
            DataHora = consulta.DataHora
        };
    }
}
