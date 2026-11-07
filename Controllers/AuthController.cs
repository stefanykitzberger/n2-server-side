using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiClinica.DTOs;
using ApiClinica.Exceptions;
using ApiClinica.Interfaces;

namespace ApiClinica.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        try
        {
            UsuarioReadDTO usuarioDTO = await _service.RegisterAsync(dto);
            return StatusCode(StatusCodes.Status201Created, usuarioDTO);
        }
        catch (RegraNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        TokenDTO? tokenDTO = await _service.LoginAsync(dto);
        if (tokenDTO == null) return Unauthorized(new { mensagem = "Email ou senha inválidos." });
        return Ok(tokenDTO);
    }

    // GET: api/auth/me
    // Retorna os dados lidos do próprio token, útil para conferir quem está autenticado e qual a role.
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            nome = User.FindFirstValue(ClaimTypes.Name),
            email = User.FindFirstValue(ClaimTypes.Email),
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}
