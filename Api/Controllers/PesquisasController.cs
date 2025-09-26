using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
[Authorize]
public class PesquisasController(IPesquisaRepositorio repo,
    ILogger<PesquisasController> logger,
    IPesquisaHandler handler,
    IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// Obtém o ‘ID’ do utilizador atual autenticado
    /// </summary>
    private Guid ObterUsuarioIdAtual()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }

    /// <summary>
    /// Obtém o nome do utilizador atual autenticado
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
    /// Obtém uma pesquisa por ‘ID’
    /// </summary>
    [HttpGet("{Id:guid}")]
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
    /// Cria uma pesquisa
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
            return Ok(retorno);
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
    [HttpPut("{Id:guid}")]
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
                    
            logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) está alterando pesquisa: {NomePesquisa}",nomeUsuario, usuarioId, comando.Nome);
            return Ok(new RetornoPadrao(true, "Pesquisa alterada com sucesso"));
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
    [HttpDelete("{Id:guid}")]
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
    [HttpPost("{PesquisaId:guid}/resultados")]
    public async Task<ActionResult> SalvarResultadoPessoa(Guid pesquisaId, [FromBody] RespostaPessoaCrud comando)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await handler.CriarResposta(pesquisaId, comando);
            return resultado.Sucesso
                ? Ok(new RetornoPadrao<RespostaPessoaView>(true, "Resultado salvo com sucesso", resultado.Data!.ConverteParaView()))
                : BadRequest(new RetornoPadrao<RespostaPessoaView>(resultado.Mensagem, resultado.Notificacoes ?? []));
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
    /// Obtem uma view para a página de resultado
    /// </summary>
    [HttpGet("{PesquisaId:guid}/resultado")]
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
            NomeDoResultadoDesejado = pesquisa.NomeDoResultadoDesejado,
            Categorias = categorias,
        };
        
        return Ok(new RetornoPadrao<PesquisaResultadoView>(true, "", view));
    }

    /// <summary>
    /// Obtém todos os resultados de uma pesquisa
    /// </summary>
    [HttpGet("{PesquisaId:guid}/resultados")]
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
    [HttpGet("{PesquisaId:guid}/resultados/{PessoaId:guid}")]
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

    [HttpDelete("{PesquisaId:guid}/resultados/{PessoaId:guid}")]
    public async Task<ActionResult> ApagarResultadoPesquisa(Guid pesquisaId, Guid pessoaId)
    {
        try
        {
            var retorno = await handler.ExcluirResultado(pesquisaId, pessoaId);
            return retorno.Sucesso ? Ok(retorno)
                : BadRequest(retorno);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter resultado da pessoa {PessoaId} na pesquisa {PesquisaId}", pessoaId,
                pesquisaId);
            return StatusCode(500, new RetornoPadrao<List<RespostaPessoaView>>("Erro interno do servidor"));
        }
    }

    [HttpGet("{PesquisaId:guid}/resultados/{PessoaId:guid}/media")]
    public async Task<IActionResult> ObterMediaPessoa(Guid pesquisaId, Guid pessoaId)
    {
        var retorno = await repo.ObtemMediaDosResultadosPorGrupoView(pessoaId);
        return Ok(new RetornoPadrao<List<MediaDeResultadosDosGruposView>>(true, "", retorno));       
    }

    /// <summary>
    /// Exporta uma pesquisa para JSON (sem dados de respostas)
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{id:guid}/exportar")]
    public async Task<IActionResult> ExportarPesquisa(Guid id)
    {
        try
        {
            var pesquisa = await repo.SelecionaAsync(id);
            if (pesquisa == null)
                return NotFound(new RetornoPadrao("Pesquisa não encontrada"));

            var usuarioId = ObterUsuarioIdAtual();
            var nomeUsuario = ObterNomeUsuarioAtual();
            
            logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) está exportando pesquisa: {NomePesquisa}", 
                nomeUsuario, usuarioId, pesquisa.Nome);

            // Converter para CRUD limpo (sem IDs internos e sem respostas)
            var exportData = new PesquisaCrud
            {
                Id = 0,
                PublicId = Guid.Empty,
                Nome = pesquisa.Nome,
                DataCriacao = pesquisa.DataCriacao,
                GruposQuestoes = pesquisa.GruposQuestoes.Select(g => new GrupoQuestaoCrud
                {
                    Id = 0,
                    PublicId = Guid.Empty,
                    PesquisaId = 0,
                    Nome = g.Nome,
                    Categoria = g.Categoria,
                    Ordem = g.Ordem,
                    Cor = g.Cor,
                    Questoes = g.Questoes.Select(q => new QuestaoCrud
                    {
                        Id = 0,
                        PublicId = Guid.Empty,
                        GrupoDeQuestaoId = 0,
                        Texto = q.Texto,
                        EInvertida = q.EInvertida,
                        Ordem = q.Ordem
                    }).ToList()
                }).ToList(),
                RespostasPessoa = [] // Vazio na exportação
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var fileName = $"{pesquisa.Nome.Replace(" ", "_")}_export_{DateTime.Now:yyyyMMdd}.json";
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            return File(bytes, "application/json", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao exportar pesquisa com ID {Id}", id);
            return StatusCode(500, new RetornoPadrao<object>("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Importa uma pesquisa de arquivo JSON
    /// </summary>
    [HttpPost("importar")]
    public async Task<IActionResult> ImportarPesquisa(IFormFile arquivo)
    {
        try
        {
            if (arquivo.Length == 0)
                return BadRequest(new RetornoPadrao<object>("Nenhum arquivo foi enviado"));

            if (!arquivo.ContentType.Contains("json") && !arquivo.FileName.EndsWith(".json"))
                return BadRequest(new RetornoPadrao<object>("Arquivo deve ser do tipo JSON"));

            var usuarioId = ObterUsuarioIdAtual();
            var nomeUsuario = ObterNomeUsuarioAtual();

            await using var stream = arquivo.OpenReadStream();
            using var reader = new StreamReader(stream);
            var jsonContent = await reader.ReadToEndAsync();

            var pesquisaImportada = JsonSerializer.Deserialize<PesquisaCrud>(jsonContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            if (pesquisaImportada == null)
                return BadRequest(new RetornoPadrao<object>("Formato de arquivo JSON inválido"));

            // Adicionar sufixo para evitar conflito de nomes
            pesquisaImportada.Nome += $" (Importado {DateTime.Now:dd/MM/yyyy HH:mm})";
            pesquisaImportada.DataCriacao = DateTime.UtcNow;

            // Limpar IDs (garantir que serão novos registros)
            pesquisaImportada.Id = 0;
            pesquisaImportada.PublicId = Guid.Empty;
            pesquisaImportada.RespostasPessoa.Clear();

            foreach (var grupo in pesquisaImportada.GruposQuestoes)
            {
                grupo.Id = 0;
                grupo.PublicId = Guid.Empty;
                grupo.PesquisaId = 0;
                
                foreach (var questao in grupo.Questoes)
                {
                    questao.Id = 0;
                    questao.PublicId = Guid.Empty;
                    questao.GrupoDeQuestaoId = 0;
                }
            }

            var retorno = await handler.Criar(pesquisaImportada);
            if (!retorno.Sucesso)
            {
                logger.LogWarning("Erro ao importar pesquisa: {Erro}", retorno.Mensagem);
                return BadRequest(retorno);
            }

            logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) importou pesquisa: {NomePesquisa}", 
                nomeUsuario, usuarioId, pesquisaImportada.Nome);

            return Ok(new RetornoPadrao<object>(true, "Pesquisa importada com sucesso", new { Id = retorno.Data!.PublicId }));
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Erro ao deserializar JSON da pesquisa");
            return BadRequest(new RetornoPadrao<object>("Formato JSON inválido", ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao importar pesquisa");
            return StatusCode(500, new RetornoPadrao<object>("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Exporta resultado individual para PDF recebendo gráfico do frontend
    /// </summary>
    [AllowAnonymous]
    [HttpPost("{pesquisaId:guid}/resultados/{pessoaId:guid}/pdf")]
    public async Task<IActionResult> ExportarResultadoPdf(Guid pesquisaId, Guid pessoaId, [FromBody] ExportarPdfCommand command)
    {
        try
        {
            // Configurar QuestPDF (necessário para licença)
            QuestPDF.Settings.License = LicenseType.Community;

            logger.LogInformation("Gerando PDF do resultado para {NomePessoa} da pesquisa {NomePesquisa}", 
                command.NomePessoa, command.NomePesquisa);

            // Converter imagem base64 para bytes
            byte[] chartImageBytes = Convert.FromBase64String(command.ChartImageBase64.Split(',').Last());

            // Gerar PDF com QuestPDF
            var pdfDocument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    
                    page.Header()
                        .PaddingBottom(20)
                        .Column(column =>
                        {
                            column.Item().AlignCenter().Text("Resultado da Avaliação")
                                .FontSize(24).FontColor(Colors.Blue.Medium).Bold();
                            column.Item().AlignCenter().Text(command.NomePessoa)
                                .FontSize(18);
                            column.Item().AlignCenter().Text($"{command.NomePesquisa} - {command.DataAvaliacao:dd/MM/yyyy HH:mm}")
                                .FontSize(14);
                        });

                    page.Content().Column(column =>
                    {
                        // Gráfico
                        column.Item().PaddingTop(20).AlignCenter()
                            .Image(chartImageBytes).FitWidth();

                        // Detalhes por categoria
                        if (command.DetalhesGrupos.Any())
                        {
                            column.Item().PaddingTop(30).Column(detailsColumn =>
                            {
                                detailsColumn.Item().Text("Detalhes por Categoria")
                                    .FontSize(16).Bold();

                                detailsColumn.Item().PaddingTop(15).Grid(grid =>
                                {
                                    grid.Columns(3);
                                    grid.Spacing(15);

                                    foreach (var grupo in command.DetalhesGrupos)
                                    {
                                        grid.Item().Border(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(15)
                                            .Column(itemColumn =>
                                            {
                                                itemColumn.Item().AlignCenter()
                                                    .Width(20).Height(20)
                                                    .Background(grupo.Cor);
                                                itemColumn.Item().AlignCenter().PaddingTop(8)
                                                    .Text(grupo.Categoria).FontSize(12).Bold();
                                                itemColumn.Item().AlignCenter().PaddingTop(5)
                                                    .Text($"{grupo.Valor:F0}%").FontSize(20).Bold()
                                                    .FontColor(Colors.Blue.Medium);
                                            });
                                    }
                                });
                            });
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .BorderTop(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .PaddingTop(10)
                        .Text($"Relatório gerado em {DateTime.Now:dd/MM/yyyy HH:mm} - PesquisaX")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });

            var pdfBytes = pdfDocument.GeneratePdf();
            var fileName = $"{command.NomePessoa}_{command.NomePesquisa}_Resultado_{DateTime.Now:yyyyMMdd}.pdf"
                .Replace(" ", "_")
                .Replace("/", "_")
                .Replace("\\", "_");

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao gerar PDF do resultado para pesquisa {PesquisaId}, pessoa {PessoaId}", 
                pesquisaId, pessoaId);
            return StatusCode(500, new RetornoPadrao<object>("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Gera ou obtém o link de compartilhamento da pesquisa
    /// </summary>
    [HttpPost("{id:guid}/compartilhar")]
    public async Task<ActionResult> GerarLinkCompartilhamento(Guid id)
    {
        try
        {
            var pesquisa = await repo.SelecionaAsync(id);
            if (pesquisa == null)
                return NotFound(new RetornoPadrao("Pesquisa não encontrada"));

            var usuarioId = ObterUsuarioIdAtual();
            var nomeUsuario = ObterNomeUsuarioAtual();

            string token;
            if (string.IsNullOrEmpty(pesquisa.TokenCompartilhamento))
            {
                token = pesquisa.GerarTokenCompartilhamento();
                await repo.AlterarAsync(pesquisa);

                logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) gerou link de compartilhamento para pesquisa: {NomePesquisa}",
                    nomeUsuario, usuarioId, pesquisa.Nome);
            }
            else
            {
                token = pesquisa.TokenCompartilhamento;
            }

            var webAppBaseUrl = configuration["WebApp:BaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";
            var linkCompartilhamento = $"{webAppBaseUrl}/compartilhado/{token}";

            return Ok(new RetornoPadrao<object>(true, "Link gerado com sucesso",
                new { Token = token, Link = linkCompartilhamento }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao gerar link de compartilhamento para pesquisa {Id}", id);
            return StatusCode(500, new RetornoPadrao("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Remove o compartilhamento da pesquisa
    /// </summary>
    [HttpDelete("{id:guid}/compartilhar")]
    public async Task<ActionResult> RemoverCompartilhamento(Guid id)
    {
        try
        {
            var pesquisa = await repo.SelecionaAsync(id);
            if (pesquisa == null)
                return NotFound(new RetornoPadrao("Pesquisa não encontrada"));

            var usuarioId = ObterUsuarioIdAtual();
            var nomeUsuario = ObterNomeUsuarioAtual();

            pesquisa.RemoverCompartilhamento();
            await repo.AlterarAsync(pesquisa);

            logger.LogInformation("Usuário {NomeUsuario} ({UsuarioId}) removeu compartilhamento da pesquisa: {NomePesquisa}",
                nomeUsuario, usuarioId, pesquisa.Nome);

            return Ok(new RetornoPadrao(true, "Compartilhamento removido com sucesso"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao remover compartilhamento da pesquisa {Id}", id);
            return StatusCode(500, new RetornoPadrao("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Acesso público à pesquisa através do token de compartilhamento
    /// </summary>
    [AllowAnonymous]
    [HttpGet("compartilhado/{token}")]
    public async Task<ActionResult> ObterPesquisaCompartilhada(string token)
    {
        try
        {
            var pesquisa = await repo.ObterPesquisaPorToken(token);
            if (pesquisa == null)
                return NotFound(new RetornoPadrao("Link de compartilhamento não encontrado ou expirado"));

            // Retornar apenas as informações necessárias para visualização
            var pesquisaCompartilhada = new
            {
                Id = pesquisa.PublicId,
                Nome = pesquisa.Nome,
                NomeDoResultadoDesejado = pesquisa.NomeDoResultadoDesejado,
                DataCriacao = pesquisa.DataCriacao,
                GruposQuestoes = pesquisa.GruposQuestoes.Select(g => new
                {
                    Nome = g.Nome,
                    Categoria = g.Categoria,
                    Cor = g.Cor,
                    Questoes = g.Questoes.Select(q => new
                    {
                        Texto = q.Texto,
                        EInvertida = q.EInvertida
                    }).ToList()
                }).ToList()
            };

            logger.LogInformation("Acesso público à pesquisa compartilhada: {NomePesquisa} via token {Token}",
                pesquisa.Nome, token);

            return Ok(new RetornoPadrao<object>(true, "", pesquisaCompartilhada));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao acessar pesquisa compartilhada com token {Token}", token);
            return StatusCode(500, new RetornoPadrao("Erro interno do servidor"));
        }
    }

}