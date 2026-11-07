using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiClinica.DTOs;
using ApiClinica.Exceptions;
using ApiClinica.Interfaces;
using ApiClinica.Models;

namespace ApiClinica.Controllers;

// Todos os endpoints exigem token JWT válido (usuário comum ou admin).
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly IConsultaService _service;

    public ConsultasController(IConsultaService service)
    {
        _service = service;
    }

    // GET: api/consultas
    [HttpGet]
    public async Task<IActionResult> GetConsultas()
    {
        List<ConsultaReadDTO> consultasDTO = await _service.GetAllAsync();
        return Ok(consultasDTO);
    }

    // GET: api/consultas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetConsultaById(int id)
    {
        ConsultaReadDTO? consultaDTO = await _service.GetByIdAsync(id);
        if (consultaDTO == null) return NotFound();
        return Ok(consultaDTO);
    }

    // POST: api/consultas
    [HttpPost]
    public async Task<IActionResult> CreateConsulta([FromBody] ConsultaCreateDTO dto)
    {
        try
        {
            ConsultaReadDTO consultaDTO = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetConsultaById), new { id = consultaDTO.Id }, consultaDTO);
        }
        catch (RegraNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // PATCH: api/consultas/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateConsulta(int id, [FromBody] ConsultaUpdateDTO dto)
    {
        try
        {
            ConsultaReadDTO? consultaDTO = await _service.UpdateAsync(id, dto);
            if (consultaDTO == null) return NotFound();
            return Ok(consultaDTO);
        }
        catch (RegraNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // DELETE: api/consultas/{id}
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConsulta(int id)
    {
        bool excluido = await _service.DeleteAsync(id);
        if (!excluido) return NotFound();
        return Ok(new { mensagem = "Consulta excluída com sucesso." });
    }
}