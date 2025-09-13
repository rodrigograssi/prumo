using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PesquisaX.Core.Repositorios;
using PesquisaX.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Net.Http.Headers;
using PesquisaX.Core.Extension;
using PesquisaX.Core.Interfaces;
using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.DTO;
using PesquisaX.Shared.DTO.Requestes;
using PesquisaX.Shared.Views;

namespace PesquisaX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IRepositorioUsuario repositorioUsuario,
    IUsuarioHandler handler,
    IConfiguration configuration,
    ILogger<AuthController> logger) : ControllerBase
{

    [HttpPost("registro")]
    public async Task<ActionResult> Registro([FromBody] UsuarioCrud comando)
    {
        if (!ModelState.IsValid)
            return BadRequest(new RetornoPadrao<UsuarioView>("Dados inválidos fornecidos"));

        var retorno = await handler.CriarUsuario(comando);
        return retorno.Sucesso
            ? Ok(new RetornoPadrao<UsuarioView>(true, "Usuario cadastrado com sucesso",
                retorno.Data!.ConverteParaView()))
            : BadRequest(retorno);
    }

    [HttpGet("has-users")]
    public async Task<ActionResult> HasUsers()
    {
        try
        {
            var hasUsers = await repositorioUsuario.ExistemUsuariosAsync();
            var retorno = new RetornoPadrao(hasUsers, hasUsers ? "Tem usuário" : "Não tem usuários");
            return Ok(retorno);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao verificar se existem usuários");
            return StatusCode(500, new RetornoPadrao<bool>("Erro ao verificar se usuarios existem", $"Erro:{ex.Message}"));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new RetornoPadrao<UsuarioView>("Erro no login", "Email e senha são obrigatórios"));

            // Buscar usuário por email
            var usuario = await repositorioUsuario.ObterPorEmailAsync(request.Email);
            if (usuario == null)
                return Unauthorized(new RetornoPadrao<UsuarioView>("Erro ao realizar login", "Email ou senha inválidos"));

            // Verificar senha
            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
            {
                logger.LogWarning("Tentativa de login com senha incorreta para: {Email}", request.Email);
                return Unauthorized(new RetornoPadrao<UsuarioView>("Erro ao realizar login", "Email ou senha inválidos"));
            }

            logger.LogInformation("Login realizado com sucesso: {Email}", request.Email);
            // Gerar token JWT
            var token = GerarTokenJwt(usuario);
            usuario.DefineAcesso(token);
            
            await repositorioUsuario.AlterarAsync(usuario);
            return Ok(new RetornoPadrao<UsuarioView>(true, "Logado com sucesso", usuario.ConverteParaView()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao realizar login: {Email}", request.Email);
            return StatusCode(500, new RetornoPadrao<bool>("Erro ao realizar login", $"Erro:{ex.Message}"));
        }
    }
    
    [HttpGet("login/token")]
    public async Task<IActionResult> LoginComToken()
    {
        // // Debug: Verificar todos os headers
        // logger.LogInformation("[DEBUG API] Headers recebidos:");
        // foreach (var header in Request.Headers)
        // {
        //     //logger.LogInformation("[DEBUG API] {Key}: {Value}", header.Key, String.Join(", ", header.Value));
        // }
        
        string authHeader = Request.Headers[HeaderNames.Authorization];
        logger.LogInformation("[DEBUG API] Authorization header: '{AuthHeader}'", authHeader ?? "NULL");
        
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized(new RetornoPadrao<UsuarioView>("Token inválido"));
            
        // Extrair apenas o token, removendo "Bearer "
        string token = authHeader.Substring("Bearer ".Length).Trim();
           
        var usuario = await repositorioUsuario.ObterPeloToken(token);
        if (usuario == null)
            return Unauthorized(new RetornoPadrao<UsuarioView>("Token inválido"));

        // Validar Token    
        if (usuario.Ativo == false)
            return Unauthorized(new RetornoPadrao<UsuarioView>("Usuário desativado"));

        // var tokenAtualido = GerarTokenJwt(usuario);
        //
        // usuario.DefineAcesso(tokenAtualido);
        // await repositorioUsuario.AlterarAsync(usuario);
        
        return Ok(new RetornoPadrao<UsuarioView>(true, "Logado com sucesso", usuario.ConverteParaView()));

    }
    
    private string GerarTokenJwt(Usuario usuario)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
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