using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Exceptions;
using ApiClinica.Interfaces;
using ApiClinica.Validators;

namespace ApiClinica.Services;

public class PacienteService : IPacienteService
{
    private readonly AppDbContext _context;
    private readonly IPacienteMapper _mapper;

    public PacienteService(AppDbContext context, IPacienteMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PacienteReadDTO>> GetAllAsync()
    {
        List<Paciente> pacientes = await _context.Pacientes.ToListAsync();
        return pacientes
            .Select(p => _mapper.ToDTO(p))
            .ToList();
    }

    public async Task<PacienteReadDTO?> GetByIdAsync(int id)
    {
        Paciente? paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return null;
        return _mapper.ToDTO(paciente);
    }

    public async Task<PacienteReadDTO> CreateAsync(PacienteCreateDTO dto)
    {
        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today)) throw new RegraNegocioException("Data de nascimento não pode ser futura.");

        if (!CpfValidator.IsValid(dto.Cpf)) throw new RegraNegocioException("CPF inválido.");

        bool cpfJaExiste = await _context.Pacientes.AnyAsync(p => p.Cpf == dto.Cpf);
        if (cpfJaExiste) throw new RegraNegocioException("Já existe um paciente com esse CPF cadastrado.");

        Paciente paciente = _mapper.ToModel(dto);
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        return _mapper.ToDTO(paciente);
    }

    public async Task<PacienteReadDTO?> UpdateAsync(int id, PacienteUpdateDTO dto)
    {
        Paciente? paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return null;

        if (dto.DataNasc != null && dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
            throw new RegraNegocioException("Data de nascimento não pode ser futura.");

        _mapper.UpdateModel(paciente, dto);
        await _context.SaveChangesAsync();

        return _mapper.ToDTO(paciente);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Paciente? paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return false;

        bool temConsultaFutura = await _context.Consultas.AnyAsync(c => c.PacienteId == id && c.DataHora > DateTime.Now);
        if (temConsultaFutura) throw new RegraNegocioException("Não é possível excluir um paciente com consultas futuras.");

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        return true;
    }
}
