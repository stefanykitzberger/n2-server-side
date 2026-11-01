using ApiClinica.Models;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class MedicoMapper : IMedicoMapper
{
    public Medico ToModel(MedicoCreateDTO dto)
    {
        return new Medico()
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            CRM = dto.CRM
        };
    }

    public void UpdateModel(Medico medico, MedicoUpdateDTO dto)
    {
        if (dto.Nome != null) medico.Nome = dto.Nome;
        if (dto.Email != null) medico.Email = dto.Email;
        if (dto.Telefone != null) medico.Telefone = dto.Telefone;
        if (dto.CRM != null) medico.CRM = dto.CRM;
    }

    public MedicoReadDTO ToDTO(Medico medico)
    {
        return new MedicoReadDTO()
        {
            Id = medico.Id,
            Nome = medico.Nome,
            Email = medico.Email,
            Telefone = medico.Telefone,
            CRM = medico.CRM
        };
    }
}
