using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;
using ApiClinica.Validators;

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
        Paciente? paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return NotFound();
        return Ok(PacienteMapper.ToDTO(paciente));
    }

    // POST: api/pacientes
    [HttpPost]
    public async Task<IActionResult> CreatePacient([FromBody] PacienteCreateDTO dto)
    {
        if (dto.DataNasc > DateOnly.FromDateTime(DateTime.Today)) return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });

        if (!CpfValidator.IsValid(dto.Cpf)) return BadRequest(new { mensagem = "CPF inválido." });

        bool cpfJaExiste = await _context.Pacientes.AnyAsync(p => p.Cpf == dto.Cpf);
        if (cpfJaExiste) return BadRequest(new { mensagem = "Já existe um paciente com esse CPF cadastrado." });

        Paciente paciente = PacienteMapper.ToModel(dto);
        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        PacienteReadDTO pacienteDTO = PacienteMapper.ToDTO(paciente);

        return CreatedAtAction(nameof(GetPacienteById), new { id = paciente.Id }, pacienteDTO);
    }

    // DELETE: api/pacientes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(int id)
    {
        Paciente? paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return NotFound();

        bool temConsultaFutura = await _context.Consultas.AnyAsync(c => c.PacienteId == id && c.DataHora > DateTime.Now);
        if (temConsultaFutura) return BadRequest(new { mensagem = "Não é possível excluir um paciente com consultas futuras." });

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Paciente excluído com sucesso." });
    }

    // PATCH: api/pacientes/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        Paciente? paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null) return NotFound();

        if (dto.DataNasc != null && dto.DataNasc > DateOnly.FromDateTime(DateTime.Today))
            return BadRequest(new { mensagem = "Data de nascimento não pode ser futura." });

        if (dto.Cpf != null && dto.Cpf != paciente.Cpf) return BadRequest(new { mensagem = "O CPF não pode ser alterado." });

        PacienteMapper.UpdateModel(paciente, dto);
        await _context.SaveChangesAsync();

        return Ok(PacienteMapper.ToDTO(paciente));
    }
}