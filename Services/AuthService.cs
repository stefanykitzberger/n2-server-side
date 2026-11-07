using ApiClinica.Models;
using ApiClinica.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApiClinica.DTOs;
using ApiClinica.Exceptions;
using ApiClinica.Interfaces;

namespace ApiClinica.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IUsuarioMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IUsuarioMapper mapper, ITokenService tokenService,
        IPasswordHasher<Usuario> passwordHasher, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<UsuarioReadDTO> RegisterAsync(RegisterDTO dto)
    {
        string email = dto.Email.Trim().ToLower();

        bool emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == email);
        if (emailJaExiste) throw new RegraNegocioException("Já existe um usuário com esse email cadastrado.");

        Usuario usuario = _mapper.ToModel(dto);
        usuario.Email = email;
        usuario.SenhaHash = _passwordHasher.HashPassword(usuario, dto.Senha);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return _mapper.ToDTO(usuario);
    }

    public async Task<TokenDTO?> LoginAsync(LoginDTO dto)
    {
        string email = dto.Email.Trim().ToLower();

        Usuario? usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        if (usuario == null) return null;

        PasswordVerificationResult resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, dto.Senha);
        if (resultado == PasswordVerificationResult.Failed) return null;

        TokenDTO tokenDTO = _tokenService.GerarToken(usuario);
        tokenDTO.Usuario = _mapper.ToDTO(usuario);
        return tokenDTO;
    }

    // Cria o usuário admin definido no appsettings.json caso ele ainda não exista (chamado no startup).
    // Pelo /register só é possível criar usuários comuns, então este é o único caminho para existir um admin.
    public async Task CriarAdminPadraoAsync()
    {
        string email = _configuration["AdminPadrao:Email"]!.Trim().ToLower();

        bool adminJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == email);
        if (adminJaExiste) return;

        Usuario admin = new Usuario()
        {
            Nome = _configuration["AdminPadrao:Nome"]!,
            Email = email,
            Role = Roles.Admin
        };
        admin.SenhaHash = _passwordHasher.HashPassword(admin, _configuration["AdminPadrao:Senha"]!);

        _context.Usuarios.Add(admin);
        await _context.SaveChangesAsync();
    }
}
