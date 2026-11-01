using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces;

public interface IMedicoMapper
{
    Medico ToModel(MedicoCreateDTO dto);

    void UpdateModel(Medico medico, MedicoUpdateDTO dto);

    MedicoReadDTO ToDTO(Medico medico);
}
