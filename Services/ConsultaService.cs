using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Exceptions;
using ApiClinica.Interfaces;

namespace ApiClinica.Services;

public class ConsultaService : IConsultaService
{
    private const int DuracaoConsultaMinutos = 30;

    private readonly AppDbContext _context;
    private readonly IConsultaMapper _mapper;

    public ConsultaService(AppDbContext context, IConsultaMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ConsultaReadDTO>> GetAllAsync()
    {
        List<Consulta> consultas = await _context.Consultas.ToListAsync();
        return consultas
            .Select(c => _mapper.ToDTO(c))
            .ToList();
    }

    public async Task<ConsultaReadDTO?> GetByIdAsync(int id)
    {
        Consulta? consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) return null;
        return _mapper.ToDTO(consulta);
    }

    public async Task<ConsultaReadDTO> CreateAsync(ConsultaCreateDTO dto)
    {
        await ValidarConsulta(dto.PacienteId, dto.MedicoId, dto.DataHora, consultaIdExcluida: null);

        Consulta consulta = _mapper.ToModel(dto);
        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        return _mapper.ToDTO(consulta);
    }

    public async Task<ConsultaReadDTO?> UpdateAsync(int id, ConsultaUpdateDTO dto)
    {
        Consulta? consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) return null;

        int pacienteId = dto.PacienteId ?? consulta.PacienteId;
        int medicoId = dto.MedicoId ?? consulta.MedicoId;
        DateTime dataHora = dto.DataHora ?? consulta.DataHora;

        bool precisaRevalidar = pacienteId != consulta.PacienteId || medicoId != consulta.MedicoId || dataHora != consulta.DataHora;
        if (precisaRevalidar)
            await ValidarConsulta(pacienteId, medicoId, dataHora, consultaIdExcluida: id);

        _mapper.UpdateModel(consulta, dto);
        await _context.SaveChangesAsync();

        return _mapper.ToDTO(consulta);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Consulta? consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) return false;

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task ValidarConsulta(int pacienteId, int medicoId, DateTime dataHora, int? consultaIdExcluida)
    {
        if (!await _context.Pacientes.AnyAsync(p => p.Id == pacienteId))
            throw new RegraNegocioException("Paciente informado não existe.");

        if (!await _context.Medicos.AnyAsync(m => m.Id == medicoId))
            throw new RegraNegocioException("Médico informado não existe.");

        if (dataHora < DateTime.Now)
            throw new RegraNegocioException("Não é possível agendar consulta no passado.");

        DateTime inicioJanela = dataHora.AddMinutes(-DuracaoConsultaMinutos);
        DateTime fimJanela = dataHora.AddMinutes(DuracaoConsultaMinutos);

        IQueryable<Consulta> consultasNaJanela = _context.Consultas.Where(c => c.DataHora > inicioJanela && c.DataHora < fimJanela);

        if (consultaIdExcluida.HasValue)
            consultasNaJanela = consultasNaJanela.Where(c => c.Id != consultaIdExcluida.Value);

        if (await consultasNaJanela.AnyAsync(c => c.MedicoId == medicoId))
            throw new RegraNegocioException("Já existe uma consulta sobreposta para este médico neste horário.");

        if (await consultasNaJanela.AnyAsync(c => c.PacienteId == pacienteId))
            throw new RegraNegocioException("Já existe uma consulta sobreposta para este paciente neste horário.");
    }
}