using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PacientesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/pacientes
    [HttpGet]
    public async Task<IActionResult> GetPacientes()
    {
        List<Paciente> pacientes = await _context.Pacientes.ToListAsync();
        List<PacienteReadDTO> pacientesDTO = pacientes
            .Select(p => PacienteMapper.ToDTO(p))
            .ToList();
        return Ok(pacientesDTO);
    }

    // GET: api/pacientes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPacienteById(int id)
    {
        Paciente paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return NotFound();
        return Ok(PacienteMapper.ToDTO(paciente));
    }

    // POST: api/pacientes
    [HttpPost]
    public async Task<IActionResult> CreatePacient([FromBody] PacienteCreateDTO dto)
    {
        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today)) return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });

        List<Paciente> pacientes = await _context.Pacientes.ToListAsync();
        Paciente paciente = PacienteMapper.ToModel(dto);
        if (pacientes.Any(p => p.Cpf == paciente.Cpf)) return BadRequest(new { mensagem = "Já existe um paciente com esse CPF cadastrado." });

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        PacienteReadDTO pacienteDTO = PacienteMapper.ToDTO(paciente);

        return CreatedAtAction(nameof(GetPacienteById), new { id = paciente.Id }, pacienteDTO);
    }

    // DELETE: api/pacientes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(int id)
    {
        Paciente paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return NotFound();

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Paciente excluído com sucesso." });
    }

    // PATCH: api/pacientes/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        Paciente paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return NotFound();

        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today)) 
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });

        if (paciente.Cpf != dto.Cpf) return BadRequest(new { mensagem = "O CPF não pode ser alterado." });

        PacienteMapper.UpdateModel(paciente, dto);
        await _context.SaveChangesAsync();

        return Ok(PacienteMapper.ToDTO(paciente));1
    }
}