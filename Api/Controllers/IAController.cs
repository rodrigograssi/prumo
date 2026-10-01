using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Prumo.Api.Services;
using Prumo.Shared.Comandos;
using Prumo.Shared.DTO;

namespace Prumo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IAController(IAIService aiService, ILogger<IAController> logger) : ControllerBase
{
    /// <summary>
    /// Verifica se o serviço de IA está disponível
    /// </summary>
    [HttpGet("disponivel")]
    public async Task<ActionResult<RetornoPadrao<bool>>> VerificarDisponibilidade()
    {
        try
        {
            var disponivel = await aiService.EstaDisponivel();
            return Ok(new RetornoPadrao<bool>(true, "", disponivel));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao verificar disponibilidade do serviço de IA");
            return Ok(new RetornoPadrao<bool>(true, "", false));
        }
    }

    /// <summary>
    /// Gera perguntas para uma categoria específica
    /// </summary>
    [HttpPost("gerar-perguntas")]
    public async Task<ActionResult<RetornoPadrao<string>>> GerarPerguntas([FromBody] GerarPerguntasCommand comando)
    {
        try
        {
            if (!await aiService.EstaDisponivel())
                return BadRequest(new RetornoPadrao<string>("Serviço de IA não está disponível"));

            if (!ModelState.IsValid)
                return BadRequest(new RetornoPadrao<string>("Dados inválidos"));

            var resultado = await aiService.GerarPerguntas(comando);

            logger.LogInformation("Perguntas geradas com IA para categoria {Categoria} sobre {Tema}",
                comando.Categoria, comando.TemaPesquisa);

            return Ok(new RetornoPadrao<string>(true, "Perguntas geradas com sucesso", resultado));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao gerar perguntas com IA");
            return StatusCode(500, new RetornoPadrao<string>("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Analisa uma pesquisa e sugere melhorias
    /// </summary>
    [HttpPost("melhorar-pesquisa")]
    public async Task<ActionResult<RetornoPadrao<string>>> MelhorarPesquisa([FromBody] MelhorarPesquisaCommand comando)
    {
        try
        {
            if (!await aiService.EstaDisponivel())
                return BadRequest(new RetornoPadrao<string>("Serviço de IA não está disponível"));

            if (!ModelState.IsValid)
                return BadRequest(new RetornoPadrao<string>("Dados inválidos"));

            var resultado = await aiService.MelhorarPesquisa(comando);

            logger.LogInformation("Sugestões de melhoria geradas com IA para pesquisa {NomePesquisa}",
                comando.PesquisaAtual.Nome);

            return Ok(new RetornoPadrao<string>(true, "Sugestões geradas com sucesso", resultado));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao gerar sugestões de melhoria com IA");
            return StatusCode(500, new RetornoPadrao<string>("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Sugere categorias e perguntas completas para um tema
    /// </summary>
    [HttpPost("sugerir-pesquisa")]
    public async Task<ActionResult<RetornoPadrao<string>>> SugerirPesquisa([FromBody] SugerirPesquisaCommand comando)
    {
        try
        {
            if (!await aiService.EstaDisponivel())
                return BadRequest(new RetornoPadrao<string>("Serviço de IA não está disponível"));

            if (!ModelState.IsValid)
                return BadRequest(new RetornoPadrao<string>("Dados inválidos"));

            var resultado = await aiService.SugerirCategoriasEPerguntas(comando.TemaPesquisa, comando.QuantidadeGrupos);

            logger.LogInformation("Estrutura de pesquisa completa gerada com IA para tema {Tema}",
                comando.TemaPesquisa);

            return Ok(new RetornoPadrao<string>(true, "Estrutura de pesquisa gerada com sucesso", resultado));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao gerar estrutura de pesquisa com IA");
            return StatusCode(500, new RetornoPadrao<string>("Erro interno do servidor"));
        }
    }
}