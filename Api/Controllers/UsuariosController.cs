using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PesquisaX.Infra.Repositorios;
using PesquisaX.Shared;
using BCrypt.Net;

namespace PesquisaX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IRepositorioUsuario repositorioUsuario, ILogger<UsuariosController> logger)
    {
        _repositorioUsuario = repositorioUsuario;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Usuario>>> ObterUsuarios([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var usuarios = await _repositorioUsuario.ListarUsuariosAsync(page, pageSize);
            
            // Remover senha do retorno
            var usuariosSemSenha = usuarios.Select(u => new Usuario
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                DataCriacao = u.DataCriacao
            }).ToList();

            return Ok(usuariosSemSenha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter usuários");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> ObterUsuario(Guid id)
    {
        try
        {
            var usuario = await _repositorioUsuario.ObterPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Remover senha do retorno
            var usuarioSemSenha = new Usuario
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                DataCriacao = usuario.DataCriacao
            };

            return Ok(usuarioSemSenha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter usuário {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> CriarUsuario([FromBody] RegistroRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se email já existe
            if (await _repositorioUsuario.EmailExisteAsync(request.Email))
            {
                return BadRequest("Email já está em uso");
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

            _logger.LogInformation("Usuário criado com sucesso: {Email}", request.Email);

            // Retornar usuário sem senha
            var usuarioSemSenha = new Usuario
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                DataCriacao = usuario.DataCriacao
            };

            return CreatedAtAction(nameof(ObterUsuario), new { id = usuario.Id }, usuarioSemSenha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário: {Email}", request.Email);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Usuario>> AtualizarUsuario(int id, [FromBody] Usuario usuarioAtualizado)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _repositorioUsuario.ObterPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Verificar se novo email já existe (exceto para o próprio usuário)
            if (usuarioAtualizado.Email != usuario.Email)
            {
                if (await _repositorioUsuario.EmailExisteAsync(usuarioAtualizado.Email))
                {
                    return BadRequest("Email já está em uso");
                }
            }

            // Atualizar dados
            usuario.Nome = usuarioAtualizado.Nome;
            usuario.Email = usuarioAtualizado.Email;

            await _repositorioUsuario.AtualizarAsync(usuario);

            _logger.LogInformation("Usuário atualizado com sucesso: {Id}", id);

            // Retornar usuário sem senha
            var usuarioSemSenha = new Usuario
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                DataCriacao = usuario.DataCriacao
            };

            return Ok(usuarioSemSenha);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> ExcluirUsuario(int id)
    {
        try
        {
            var usuario = await _repositorioUsuario.ObterPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Verificar se não é o último usuário
            var totalUsuarios = await _repositorioUsuario.ContarUsuariosAsync();
            if (totalUsuarios <= 1)
            {
                return BadRequest("Não é possível excluir o último usuário do sistema");
            }

            await _repositorioUsuario.ExcluirAsync(id);

            _logger.LogInformation("Usuário excluído com sucesso: {Id}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir usuário: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("{id}/reset-password")]
    public async Task<ActionResult> ResetarSenha(Guid id, [FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _repositorioUsuario.ObterPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Criar hash da nova senha
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            usuario.Senha = senhaHash;

            await _repositorioUsuario.AtualizarAsync(usuario);

            _logger.LogInformation("Senha resetada com sucesso para usuário: {Id}", id);

            return Ok("Senha resetada com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao resetar senha do usuário: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> ContarUsuarios()
    {
        try
        {
            var count = await _repositorioUsuario.ContarUsuariosAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao contar usuários");
            return StatusCode(500, 0);
        }
    }
}