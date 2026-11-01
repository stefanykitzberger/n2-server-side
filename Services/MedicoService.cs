using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Exceptions;
using ApiClinica.Interfaces;

namespace ApiClinica.Services;

public class MedicoService : IMedicoService
{
    private readonly AppDbContext _context;
    private readonly IMedicoMapper _mapper;

    public MedicoService(AppDbContext context, IMedicoMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<MedicoReadDTO>> GetAllAsync()
    {
        List<Medico> medicos = await _context.Medicos.ToListAsync();
        return medicos
            .Select(m => _mapper.ToDTO(m))
            .ToList();
    }

    public async Task<MedicoReadDTO?> GetByIdAsync(int id)
    {
        Medico? medico = await _context.Medicos.FindAsync(id);
        if (medico == null) return null;
        return _mapper.ToDTO(medico);
    }

    public async Task<MedicoReadDTO> CreateAsync(MedicoCreateDTO dto)
    {
        Medico medico = _mapper.ToModel(dto);
        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        return _mapper.ToDTO(medico);
    }

    public async Task<bool> UpdateAsync(int id, MedicoUpdateDTO dto)
    {
        Medico? medico = await _context.Medicos.FindAsync(id);
        if (medico == null) return false;

        _mapper.UpdateModel(medico, dto);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Medico? medico = await _context.Medicos.FindAsync(id);
        if (medico == null) return false;

        bool temConsultaFutura = await _context.Consultas.AnyAsync(c => c.MedicoId == id && c.DataHora > DateTime.Now);
        if (temConsultaFutura) throw new RegraNegocioException("Não é possível excluir um médico com consultas futuras.");

        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();

        return true;
    }
}
