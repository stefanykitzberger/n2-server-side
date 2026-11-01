using ApiClinica.Models;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class PacienteMapper : IPacienteMapper
{
    public Paciente ToModel(PacienteCreateDTO dto)
    {
        return new Paciente()
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            DataNasc = dto.DataNasc,
            Cpf = dto.Cpf
        };
    }

    public void UpdateModel(Paciente paciente, PacienteUpdateDTO dto)
    {
        if (dto.Nome != null) paciente.Nome = dto.Nome;
        if (dto.Email != null) paciente.Email = dto.Email;
        if (dto.Telefone != null) paciente.Telefone = dto.Telefone;
        if (dto.DataNasc != null) paciente.DataNasc = dto.DataNasc.Value;
    }

    public PacienteReadDTO ToDTO(Paciente paciente)
    {
        return new PacienteReadDTO()
        {
            Id = paciente.Id,
            Nome = paciente.Nome,
            Email = paciente.Email,
            Telefone = paciente.Telefone,
            DataNasc = paciente.DataNasc
        };
    }
}
