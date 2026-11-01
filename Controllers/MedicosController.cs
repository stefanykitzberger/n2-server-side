using Microsoft.AspNetCore.Mvc;
using ApiClinica.DTOs;
using ApiClinica.Exceptions;
using ApiClinica.Interfaces;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _service;

    public MedicosController(IMedicoService service)
    {
        _service = service;
    }

    // GET: api/medicos
    [HttpGet]
    public async Task<IActionResult> GetMedicos()
    {
        List<MedicoReadDTO> medicosDTO = await _service.GetAllAsync();
        return Ok(medicosDTO);
    }

    // GET: api/medicos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMedicoById(int id)
    {
        MedicoReadDTO? medicoDTO = await _service.GetByIdAsync(id);
        if (medicoDTO == null) return NotFound();
        return Ok(medicoDTO);
    }

    // POST: api/medicos
    [HttpPost]
    public async Task<IActionResult> CreateMedico([FromBody] MedicoCreateDTO dto)
    {
        MedicoReadDTO medicoDTO = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetMedicoById), new { id = medicoDTO.Id }, medicoDTO);
    }
    
    // PATCH: api/medicos/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateMedico(int id, [FromBody] MedicoUpdateDTO dto)
    {
        bool atualizado = await _service.UpdateAsync(id, dto);
        if (!atualizado) return NotFound();
        return NoContent();
    }

    // DELETE: api/medicos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedico(int id)
    {
        try
        {
            bool excluido = await _service.DeleteAsync(id);
            if (!excluido) return NotFound();
            return Ok(new { mensagem = "Médico excluído com sucesso." });
        }
        catch (RegraNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
