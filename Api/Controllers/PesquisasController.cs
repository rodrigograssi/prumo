using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PesquisaX.Infra.Repositorios;
using PesquisaX.Shared.Modelos;
using System.Security.Claims;

namespace PesquisaX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PesquisasController : ControllerBase
{
    private readonly IRepositorioPesquisa _repositorioPesquisa;
    private readonly ILogger<PesquisasController> _logger;

    public PesquisasController(IRepositorioPesquisa repositorioPesquisa, ILogger<PesquisasController> logger)
    {
        _repositorioPesquisa = repositorioPesquisa;
        _logger = logger;
    }

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
            
            _logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) solicitou todas as pesquisas", nomeUsuario, usuarioId);
            
            var pesquisas = await _repositorioPesquisa.ObterTodosAsync();
            return Ok(pesquisas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as pesquisas para usuário {UsuarioId}", ObterUsuarioIdAtual());
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém uma pesquisa por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Pesquisa>> ObterPesquisaPorId(Guid id)
    {
        try
        {
            var pesquisa = await _repositorioPesquisa.ObterPorIdAsync(id);
            if (pesquisa == null)
                return NotFound($"Pesquisa com ID {id} não encontrada");

            return Ok(pesquisa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pesquisa com ID {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Cria uma nova pesquisa
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Pesquisa>> CriarPesquisa([FromBody] Pesquisa pesquisa)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioId = ObterUsuarioIdAtual();
            var nomeUsuario = ObterNomeUsuarioAtual();
            
            _logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) está criando nova pesquisa: {NomePesquisa}", 
                nomeUsuario, usuarioId, pesquisa.Nome);

            var pesquisaCriada = await _repositorioPesquisa.AdicionarAsync(pesquisa);
            
            _logger.LogInformation("Pesquisa {PesquisaId} criada com sucesso por {NomeUsuario} ({UsuarioId})", 
                pesquisaCriada.Id, nomeUsuario, usuarioId);
                
            return CreatedAtAction(nameof(ObterPesquisaPorId), new { id = pesquisaCriada.Id }, pesquisaCriada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pesquisa para usuário {UsuarioId}", ObterUsuarioIdAtual());
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Atualiza uma pesquisa existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPesquisa(Guid id, [FromBody] Pesquisa pesquisa)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != pesquisa.Id)
                return BadRequest("ID da URL não confere com ID da pesquisa");

            // Verificação de existência sem carregar a entidade para não criar conflito de tracking
            var pesquisaExistente = await _repositorioPesquisa.ObterPorIdAsync(id); 
            if (pesquisaExistente == null)
                return NotFound($"Pesquisa com ID {id} não encontrada");

            await _repositorioPesquisa.ExcluirAsync(pesquisa.Id);
            await _repositorioPesquisa.AdicionarAsync(pesquisa);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar pesquisa com ID {Id}", id);
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
            var pesquisaExistente = await _repositorioPesquisa.ExisteAsync(id);
            if (!pesquisaExistente)
                return NotFound($"Pesquisa com ID {id} não encontrada");

            await _repositorioPesquisa.ExcluirAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir pesquisa com ID {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Salva resultado de uma pessoa na pesquisa
    /// </summary>
    [HttpPost("{pesquisaId}/resultados")]
    public async Task<ActionResult<ResultadoPessoa>> SalvarResultadoPessoa(Guid pesquisaId, [FromBody] ResultadoPessoa resultado)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if(resultado.PesquisaId != pesquisaId)
                resultado.PesquisaId = pesquisaId;

            var resultadoSalvo = await _repositorioPesquisa.AdicionarResultadoAsync(resultado);
            return Ok(resultadoSalvo);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Pesquisa não encontrada: {PesquisaId}", pesquisaId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar resultado para pesquisa {PesquisaId}", pesquisaId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém todos os resultados de uma pesquisa
    /// </summary>
    [HttpGet("{pesquisaId}/resultados")]
    public async Task<ActionResult<IEnumerable<ResultadoPessoa>>> ObterResultadosPesquisa(Guid pesquisaId)
    {
        try
        {
            var resultados = await _repositorioPesquisa.ObterResultadosPesquisaAsync(pesquisaId);
            return Ok(resultados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter resultados da pesquisa {PesquisaId}", pesquisaId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém resultado específico de uma pessoa na pesquisa
    /// </summary>
    [HttpGet("{pesquisaId}/resultados/{pessoaId}")]
    public async Task<ActionResult<ResultadoPessoa>> ObterResultadoPessoa(Guid pesquisaId, Guid pessoaId)
    {
        try
        {
            var resultado = await _repositorioPesquisa.ObterResultadoPessoaAsync(pesquisaId, pessoaId);
            if (resultado == null)
                return NotFound($"Resultado não encontrado para pessoa {pessoaId} na pesquisa {pesquisaId}");

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter resultado da pessoa {PessoaId} na pesquisa {PesquisaId}", pessoaId, pesquisaId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}