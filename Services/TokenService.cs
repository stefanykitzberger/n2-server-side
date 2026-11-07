using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;
using ApiClinica.Models;

namespace ApiClinica.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenDTO GerarToken(Usuario usuario)
    {
        // Claims: informações do usuário que vão "dentro" do token. A Role é usada pelo [Authorize(Roles = ...)].
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        // Assina o token com a chave secreta (HMAC SHA-256), assim a API consegue validar que ele não foi alterado.
        SymmetricSecurityKey chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        SigningCredentials credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        DateTime expiraEm = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiracaoEmMinutos"));

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new TokenDTO()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraEm = expiraEm
        };
    }
}
