using ApiClinica.DTOs;

namespace ApiClinica.Interfaces;

public interface IAuthService
{
    Task<UsuarioReadDTO> RegisterAsync(RegisterDTO dto);

    Task<TokenDTO?> LoginAsync(LoginDTO dto);

    Task CriarAdminPadraoAsync();
}
