using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PesquisaX.Core.Repositorios;
using PesquisaX.Core.Modelos;
using System.Security.Claims;
using PesquisaX.Core.Extension;
using PesquisaX.Core.Interfaces;
using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.DTO;
using PesquisaX.Shared.Views;

namespace PesquisaX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class PesquisasController(IPesquisaRepositorio repo,
    ILogger<PesquisasController> logger, 
    IPesquisaHandler handler) : ControllerBase
{

    /// <summary>
    /// Obtém o ID do usuário atual autenticado
    /// </summary>
    private Guid ObterUsuarioIdAtual()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }

    /// <summary>
    /// Obtém o nome do usuário atual autenticado
    /// </summary>
    private string ObterNomeUsuarioAtual()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuário";
    }

    /// <summary>
    /// Obtém todas as pesquisas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pesquisa>>> ObterTodasPesquisas()
    {
        try
        {
            var usuarioId = ObterUsuarioIdAtual();
            var nomeUsuario = ObterNomeUsuarioAtual();
            
            logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) solicitou todas as pesquisas", nomeUsuario, usuarioId);
            
            var pesquisas = await repo.ObterPequisaListView();
            return Ok(new RetornoPadrao<List<PesquisaListaView>>(true, "", pesquisas));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter todas as pesquisas para usuário {UsuarioId}", ObterUsuarioIdAtual());
            return StatusCode(500,new RetornoPadrao<List<PesquisaListaView>>("Erro interno no sistema", ex.Message));
        }
    }

    /// <summary>
    /// Obtém uma pesquisa por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> ObterPesquisaPorId(Guid id)
    {
        try
        {
            var pesquisa = await repo.SelecionaAsync(id);
            if (pesquisa == null)
                return NotFound($"Pesquisa com ID {id} não encontrada");

            return Ok(new RetornoPadrao<PesquisaCrud>(true, "", pesquisa.ConverteParaCrud()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter pesquisa com ID {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Cria uma nova pesquisa
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CriarPesquisa([FromBody] PesquisaCrud comando)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new RetornoPadrao("Dados inválidos"));

            var usuarioId = ObterUsuarioIdAtual();
            var nomeUsuario = ObterNomeUsuarioAtual();

            var retorno = await handler.Criar(comando);
            if (!retorno.Sucesso)
            {
                logger.LogWarning("Não foi possível criar a pesquisa: {Erro} - {Mensagem}", retorno.Mensagem,
                    retorno.Notificacoes);
                return BadRequest(retorno);
            }

            logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) está criando nova pesquisa: {NomePesquisa}",nomeUsuario, usuarioId, comando.Nome);
            return CreatedAtAction(nameof(AtualizarPesquisa),
                new { id = retorno.Data!.Id },
                new RetornoPadrao(true, "Pesquisa criada com sucesso"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar pesquisa para usuário {UsuarioId}", ObterUsuarioIdAtual());
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Atualiza uma pesquisa existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPesquisa(Guid id, [FromBody] PesquisaCrud comando)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioId = ObterUsuarioIdAtual();
            var nomeUsuario = ObterNomeUsuarioAtual();

            var retorno = await handler.Alterar(comando);
            if (!retorno.Sucesso)
                logger.LogWarning("Não foi possível alterar a pesquisa: {Erro} - {Mensagem}", retorno.Mensagem, retorno.Notificacoes);
                    
            logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) está alteranod pesquisa: {NomePesquisa}",nomeUsuario, usuarioId, comando.Nome);
            return Ok(new RetornoPadrao(true, "Pesquisa alterada com sucesso"));;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar pesquisa para usuário {UsuarioId}", ObterUsuarioIdAtual());
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Exclui uma pesquisa
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirPesquisa(Guid id)
    {
        try
        {

            var retorno = await handler.Excluir(id);
            return retorno.Sucesso
                ? NoContent()
                : BadRequest(retorno);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao excluir pesquisa com ID {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Salva resultado de uma pessoa na pesquisa
    /// </summary>
    [HttpPost("{pesquisaId}/resultados")]
    public async Task<ActionResult> SalvarResultadoPessoa(Guid pesquisaId, [FromBody] RespostaPessoaCrud comando)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await handler.CriarResposta(pesquisaId, comando);
            return resultado.Sucesso
                ? Ok(new RetornoPadrao<RespostaPessoaView>(true, "Resultado salvo com sucesso", resultado.Data!.ConverteParaView()))
                : BadRequest(new RetornoPadrao<RespostaPessoaView>(resultado.Mensagem, resultado.Notificacoes ?? []));;
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Pesquisa não encontrada: {PesquisaId}", pesquisaId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao salvar resultado para pesquisa {PesquisaId}", pesquisaId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtem uma view para a pagina de resultado
    /// </summary>
    [HttpGet("{pesquisaId}/resultado")]
    public async Task<ActionResult> ObterResultadoPesquisa(Guid pesquisaId)
    {
        var pesquisa = await repo.SelecionaAsync(pesquisaId);
        if (pesquisa == null)
        {
            return BadRequest(new RetornoPadrao<PesquisaResultadoView>("Pesquisa não localizada"));
        }
        
        var categorias = pesquisa.GruposQuestoes.Select(gr =>
            new CategoriaView() { Nome = gr.Categoria, Cor = gr.Cor}
        
        ).ToList();
        var view = new PesquisaResultadoView()
        {
            Nome = pesquisa.Nome,
            Categorias = categorias,
        };
        
        return Ok(new RetornoPadrao<PesquisaResultadoView>(true, "", view));
    }

    /// <summary>
    /// Obtém todos os resultados de uma pesquisa
    /// </summary>
    [HttpGet("{pesquisaId}/resultados")]
    public async Task<ActionResult> ObterResultadosPesquisa(Guid pesquisaId)
    {
        try
        {
            var id = await repo.ObtemPesquisaId(pesquisaId);
            var grupos = await repo.ObtemGruposDePergunta(id);
            var resultados = await repo.ObterResultadosPesquisaAsync(id);

            var view = resultados.Select(r => r.ConverteParaView(grupos)).ToList();
            var retorno = new RetornoPadrao<List<RespostaPessoaView>>(true, "", view);
            
            return Ok(retorno);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter resultados da pesquisa {PesquisaId}", pesquisaId);
            return StatusCode(500, new RetornoPadrao<List<RespostaPessoaView>>("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Obtém resultado específico de uma pessoa na pesquisa
    /// </summary>
    [HttpGet("{pesquisaId}/resultados/{pessoaId}")]
    public async Task<ActionResult> ObterResultadoPessoa(Guid pesquisaId, Guid pessoaId)
    {
        try
        {
            var resultado = await repo.ObterResultadoPessoaAsync(pesquisaId, pessoaId);
            if (resultado == null)
                return BadRequest(new RetornoPadrao<RespostaPessoaView>("Resultado não localizado"));

            var questoes = await repo.ObtemGruposDePergunta(resultado.PesquisaId);
            
            var view = resultado.ConverteParaView(questoes);
            return Ok(new RetornoPadrao<RespostaPessoaView>(true, "", view));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter resultado da pessoa {PessoaId} na pesquisa {PesquisaId}", pessoaId, pesquisaId);
            return StatusCode(500, new RetornoPadrao<List<RespostaPessoaView>>("Erro interno do servidor"));
        }
    }

    [HttpGet("{PesquisaId}/resultados/{PersonId}/media")]
    public async Task<IActionResult> ObterMediaPessoa(Guid PesquisaId, Guid PersonId)
    {
        var retorno = await repo.ObtemMediaDosResultadosPorGrupoView(PersonId);
        return Ok(new RetornoPadrao<List<MediaDeResultadosDosGruposView>>(true, "", retorno));       
    }

   
}