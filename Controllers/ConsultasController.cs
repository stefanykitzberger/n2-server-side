using Microsoft.AspNetCore.Mvc;
using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Mappers;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private const int DuracaoConsultaMinutos = 30;
    private readonly AppDbContext _context;

    public ConsultasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/consultas
    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        List<Consulta> consultas = await _context.Consultas.ToListAsync();
        List<ConsultaReadDTO> consultasDTO = consultas
            .Select(c => ConsultaMapper.ToDTO(c))
            .ToList();
        return Ok(consultasDTO);
    }

    // GET: api/consultas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        Consulta consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) return NotFound();
        return Ok(ConsultaMapper.ToDTO(consulta));
    }

    // POST: api/consultas
    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO dto)
    {
        string? erro = await ValidarConsulta(dto.PacienteId, dto.MedicoId, dto.DataHora, consultaIdExcluida: null);
        if (erro != null) return BadRequest(new { mensagem = erro });

        Consulta consulta = ConsultaMapper.ToModel(dto);
        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        ConsultaReadDTO consultaDTO = ConsultaMapper.ToDTO(consulta);

        return CreatedAtAction(nameof(GetConsultaById), new { id = consulta.Id }, consultaDTO);
    }

    // DELETE: api/consultas/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConsulta(int id)
    {
        Consulta consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) return NotFound();

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Consulta excluída com sucesso." });
    }

    // PATCH: api/consultas/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO dto)
    {
        Consulta consulta = await _context.Consultas.FindAsync(id);
        if (consulta == null) return NotFound();

        bool precisaRevalidar = dto.PacienteId != consulta.PacienteId || dto.MedicoId != consulta.MedicoId || dto.DataHora != consulta.DataHora;
        if (precisaRevalidar)
        {
            string? erro = await ValidarConsulta(dto.PacienteId, dto.MedicoId, dto.DataHora, consultaIdExcluida: id);
            if (erro != null) return BadRequest(new { mensagem = erro });
        }

        ConsultaMapper.UpdateModel(consulta, dto);
        await _context.SaveChangesAsync();

        return Ok(ConsultaMapper.ToDTO(consulta));
    }

    private async Task<string?> ValidarConsulta(int pacienteId, int medicoId, DateTime dataHora, int? consultaIdExcluida)
    {
        if (!await _context.Pacientes.AnyAsync(p => p.Id == pacienteId)) 
            return "Paciente informado não existe.";

        if (!await _context.Medicos.AnyAsync(m => m.Id == medicoId)) 
            return "Médico informado não existe.";

        if (dataHora < DateTime.Now) 
            return "Não é possível agendar consulta no passado.";

        DateTime inicioJanela = dataHora.AddMinutes(-DuracaoConsultaMinutos);
        DateTime fimJanela = dataHora.AddMinutes(DuracaoConsultaMinutos);

        IQueryable<Consulta> consultasNaJanela = _context.Consultas.Where(c => c.DataHora > inicioJanela && c.DataHora < fimJanela);

        if (consultaIdExcluida.HasValue)
            consultasNaJanela = consultasNaJanela.Where(c => c.Id != consultaIdExcluida.Value);

        if (await consultasNaJanela.AnyAsync(c => c.MedicoId == medicoId))
            return "Já existe uma consulta sobreposta para este médico neste horário.";

        if (await consultasNaJanela.AnyAsync(c => c.PacienteId == pacienteId))
            return "Já existe uma consulta sobreposta para este paciente neste horário.";

        return null;
    }
}