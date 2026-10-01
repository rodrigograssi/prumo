using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prumo.Core.Repositorios;
using Prumo.Core;
using BCrypt.Net;
using Microsoft.Net.Http.Headers;
using Prumo.Core.Extension;
using Prumo.Core.Interfaces;
using Prumo.Shared.Comandos;
using Prumo.Shared.DTO;
using Prumo.Shared.DTO.Requestes;
using Prumo.Shared.DTO.Responses;
using Prumo.Shared.Views;

namespace Prumo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController(IRepositorioUsuario repositorioUsuario, IUsuarioHandler handler, ILogger<UsuariosController> logger) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult> ObterUsuarios([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var usuarios = await repositorioUsuario.ListarUsuariosAsync(page, pageSize);
            var usuariosView = usuarios.Select(u => u.ConverteParaView()).ToList();   
            // Remover senha do retorno
            return Ok(new RetornoPadrao<List<UsuarioView>>(true, "Listagem de usuários", usuariosView));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter usuários");
            return StatusCode(500, new RetornoPadrao<List<UsuarioView>>("Erro interno no sistema", $"Erro: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> ObterUsuario(Guid id)
    {
        try
        {
            var usuario = await repositorioUsuario.ObterPorIdAsync(id);
            if (usuario == null)
                return NotFound(new RetornoPadrao<UsuarioView>("Erro ao obter usuário", "Usuário não localizado"));
          
            return Ok(new RetornoPadrao<UsuarioView>(true, "Usuário localizado", usuario.ConverteParaView()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter usuário {Id}", id);
            return StatusCode(500, new RetornoPadrao<UsuarioView>("Erro interno no sistema", $"Erro: {ex.Message}"));
        }
    }

    [HttpPost]
    public async Task<ActionResult> CriarUsuario([FromBody] UsuarioCrud comando)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var retorno = await handler.CriarUsuario(comando);
            return retorno.Sucesso ?
                CreatedAtAction(
                    nameof(ObterUsuario),
                    new { id = retorno.Data!.Id },
                    new RetornoPadrao<UsuarioView>(true, "Usuário criado com sucesso", retorno.Data.ConverteParaView()))
                : BadRequest(new RetornoPadrao<UsuarioView>(retorno.Mensagem, retorno.Notificacoes ?? []));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar usuário: {Email}", comando.Email);
            return StatusCode(500, new RetornoPadrao<UsuarioView>("Erro interno no sistema", $"Erro: {ex.Message}"));
        }
    }

    [HttpPut("{id:Guid}")]
    public async Task<ActionResult<Usuario>> AtualizarUsuario(Guid id, [FromBody] UsuarioCrud comando)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new RetornoPadrao("Erro ao alterar usuario", "Dados inválido"));

            var retorno = await handler.AlterarUsuario(comando);
            return retorno.Sucesso
                ? Ok(new RetornoPadrao(true, "Usuário Alterado com sucesso"))
                : BadRequest(new RetornoPadrao(retorno.Mensagem, retorno.Notificacoes ?? []));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao atualizar usuário: {Id}", id);
            return StatusCode(500, new RetornoPadrao("Erro interno no sistema", $"Erro: {ex.Message}"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> ExcluirUsuario(Guid id)
    {
        try
        {
            var retorno = await handler.ExcluirUsuarioAsync(id);
            return retorno.Sucesso 
                ? Ok(retorno) 
                : BadRequest(retorno);
            
           
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao excluir usuário: {Id}", id);
            return StatusCode(500, new RetornoPadrao("Erro interno no sistema", $"Erro: {ex.Message}"));
        }
    }

    [HttpPost("{id:Guid}/reset-password")]
    public async Task<ActionResult> ResetarSenha(Guid id, [FromBody] ResetPasswordRequest comando)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new RetornoPadrao("Erro ao redefinir a senha", "Dados inválidos"));

            var retorno = await handler.ResetarSenha(id,comando);
            return retorno.Sucesso
                ? Ok(retorno)
                : BadRequest(retorno);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao resetar senha do usuário: {Id}", id);
            return StatusCode(500, new RetornoPadrao("Erro interno no sistema", $"Erro: {ex.Message}"));
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> ContarUsuarios()
    {
        try
        {
            var count = await repositorioUsuario.ContarUsuariosAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao contar usuários");
            return StatusCode(500, 0);
        }
    }
    
   
}