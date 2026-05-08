using PousadaApi.Application.Interfaces;
using PousadaApi.Domain.Entities;
using PousadaApi.Domain.Interfaces;

namespace PousadaApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario> ResgistrarAsync(string nome, string email, string senha, string perfil = "Gerente")
    {
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuarioExistente != null)
            throw new InvalidOperationException("Email já cadastrado");

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            SenhaHash = _passwordHasher.Hash(senha),
            Perfil = perfil
        };

        await _usuarioRepository.AdicionarAsync(usuario);
        return usuario;
    }

    public async Task<Usuario> AutenticarAsync(string email, string senha)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

        if (usuario == null || !_passwordHasher.Verify(senha, usuario.SenhaHash))
            throw new InvalidOperationException("Email ou senha inválidos");

        return usuario;
    }

    public string GerarToken(Usuario usuario) => _jwtTokenGenerator.Generate(usuario);
}
