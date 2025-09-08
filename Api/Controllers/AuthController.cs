using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PesquisaX.Core.Repositorios;
using PesquisaX.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using PesquisaX.Core.Extension;
using PesquisaX.Shared.DTO.Requestes;
using PesquisaX.Shared.DTO.Responses;

namespace PesquisaX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IRepositorioUsuario repositorioUsuario, 
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _repositorioUsuario = repositorioUsuario;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("registro")]
    public async Task<ActionResult<LoginResponse>> Registro([FromBody] RegistroRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos fornecidos"
                });
            }

            // Verificar se o email já existe
            if (await _repositorioUsuario.EmailExisteAsync(request.Email))
            {
                return BadRequest(new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Email já está em uso"
                });
            }

            // Criar hash da senha
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            // Criar usuário
            var usuario = new Usuario
            {
                Nome = request.Nome,
                Email = request.Email,
                Senha = senhaHash,
                DataCriacao = DateTime.UtcNow
            };

            await _repositorioUsuario.AdicionarAsync(usuario);

            _logger.LogInformation("Usuário registrado com sucesso: {Email}", request.Email);

            // Gerar token JWT
            var token = GerarTokenJwt(usuario);

            return Ok(new LoginResponse
            {
                Sucesso = true,
                Mensagem = "Usuário registrado com sucesso",
                Token = token,
                Usuario = usuario.ConverteParaView()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário: {Email}", request.Email);
            return StatusCode(500, new LoginResponse
            {
                Sucesso = false,
                Mensagem = "Erro interno do servidor"
            });
        }
    }

    [HttpGet("has-users")]
    public async Task<ActionResult<bool>> HasUsers()
    {
        try
        {
            var hasUsers = await _repositorioUsuario.ExistemUsuariosAsync();
            return Ok(hasUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se existem usuários");
            return StatusCode(500, false);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Email e senha são obrigatórios"
                });
            }

            // Buscar usuário por email
            var usuario = await _repositorioUsuario.ObterPorEmailAsync(request.Email);
            if (usuario == null)
            {
                return Unauthorized(new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Email ou senha inválidos"
                });
            }

            // Verificar senha
            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
            {
                _logger.LogWarning("Tentativa de login com senha incorreta para: {Email}", request.Email);
                return Unauthorized(new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Email ou senha inválidos"
                });
            }

            _logger.LogInformation("Login realizado com sucesso: {Email}", request.Email);

            // Gerar token JWT
            var token = GerarTokenJwt(usuario);

            return Ok(new LoginResponse
            {
                Sucesso = true,
                Mensagem = "Login realizado com sucesso",
                Token = token,
                Usuario = usuario.ConverteParaView()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar login: {Email}", request.Email);
            return StatusCode(500, new LoginResponse
            {
                Sucesso = false,
                Mensagem = "Erro interno do servidor"
            });
        }
    }

    private string GerarTokenJwt(Usuario usuario)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "PesquisaXSecretKeyPadrao123456789!@#$%^&*()";
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7), // Token válido por 7 dias
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}