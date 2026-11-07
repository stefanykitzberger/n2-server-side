using ApiClinica.DTOs;
using ApiClinica.Models;

namespace ApiClinica.Interfaces;

public interface IUsuarioMapper
{
    Usuario ToModel(RegisterDTO dto);

    UsuarioReadDTO ToDTO(Usuario usuario);
}
