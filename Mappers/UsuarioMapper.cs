using ApiClinica.Models;
using ApiClinica.DTOs;
using ApiClinica.Interfaces;

namespace ApiClinica.Mappers;

public class UsuarioMapper : IUsuarioMapper
{
    // O hash da senha é gerado no AuthService; todo usuário registrado nasce como Comum.
    public Usuario ToModel(RegisterDTO dto)
    {
        return new Usuario()
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Role = Roles.Comum
        };
    }

    public UsuarioReadDTO ToDTO(Usuario usuario)
    {
        return new UsuarioReadDTO()
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Role = usuario.Role
        };
    }
}
