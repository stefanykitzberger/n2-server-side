using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/medicos
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        List<Medico> medicos = await _context.Medicos.ToListAsync();
        List<MedicoReadDTO> medicosDTO = medicos
            .Select(m => MedicoMapper.ToDTO(m))
            .ToList();
        return Ok(medicosDTO);
    }

    // GET: api/medicos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        Medico medico = await _context.Medicos.FindAsync(id);
        if (medico == null) return NotFound();
        return Ok(MedicoMapper.ToDTO(medico));
    }

    // POST: api/medicos
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] MedicoCreateDTO dto)
    {
        Medico medico = MedicoMapper.ToModel(dto);
        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync();

        MedicoReadDTO medicoDTO = MedicoMapper.ToDTO(medico);

        return CreatedAtAction(nameof(GetMedicoById), new { id = medico.Id }, medicoDTO);
    }
    
    // PATCH: api/medicos/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoUpdateDTO dto)
    {
        Medico medico = await _context.Medicos.FindAsync(id);
        if (medico == null) return NotFound();
        MedicoMapper.Update(medico, dto);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/medicos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedico(int id)
    {
        Medico medico = await _context.Medicos.FindAsync(id);
        if (medico == null) return NotFound();

        bool temConsultaFutura = await _context.Consultas.AnyAsync(c => c.MedicoId == id && c.DataHora > DateTime.Now);
        if (temConsultaFutura) return BadRequest(new { mensagem = "Não é possível excluir um médico com consultas futuras." });

        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();
        return Ok(new { mensagem = "Médico excluído com sucesso." });
    }
}