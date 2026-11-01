using Microsoft.AspNetCore.Mvc;
using ApiClinica.DTOs;
using ApiClinica.Exceptions;
using ApiClinica.Interfaces;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

    // GET: api/pacientes
    [HttpGet]
    public async Task<IActionResult> GetPacientes()
    {
        List<PacienteReadDTO> pacientesDTO = await _service.GetAllAsync();
        return Ok(pacientesDTO);
    }

    // GET: api/pacientes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPacienteById(int id)
    {
        PacienteReadDTO? pacienteDTO = await _service.GetByIdAsync(id);
        if (pacienteDTO == null) return NotFound();
        return Ok(pacienteDTO);
    }

    // POST: api/pacientes
    [HttpPost]
    public async Task<IActionResult> CreatePacient([FromBody] PacienteCreateDTO dto)
    {
        try
        {
            PacienteReadDTO pacienteDTO = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPacienteById), new { id = pacienteDTO.Id }, pacienteDTO);
        }
        catch (RegraNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // DELETE: api/pacientes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(int id)
    {
        try
        {
            bool excluido = await _service.DeleteAsync(id);
            if (!excluido) return NotFound();
            return Ok(new { mensagem = "Paciente excluído com sucesso." });
        }
        catch (RegraNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // PATCH: api/pacientes/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDTO dto)
    {
        try
        {
            PacienteReadDTO? pacienteDTO = await _service.UpdateAsync(id, dto);
            if (pacienteDTO == null) return NotFound();
            return Ok(pacienteDTO);
        }
        catch (RegraNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
