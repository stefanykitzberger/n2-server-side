using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces;

public interface IPacienteMapper
{
    Paciente ToModel(PacienteCreateDTO dto);

    void UpdateModel(Paciente paciente, PacienteUpdateDTO dto);

    PacienteReadDTO ToDTO(Paciente paciente);
}
