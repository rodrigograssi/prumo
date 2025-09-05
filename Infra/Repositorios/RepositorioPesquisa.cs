using Microsoft.EntityFrameworkCore;
using PesquisaX.Infra.Contexto;
using PesquisaX.Shared.Modelos;

namespace PesquisaX.Infra.Repositorios;

public class RepositorioPesquisa : RepositorioBase<Pesquisa>, IRepositorioPesquisa
{
    public RepositorioPesquisa(PesquisaXContexto contexto) : base(contexto)
    {
    }

    public async Task<Pesquisa?> ObterPesquisaCompletaAsync(Guid id)
    {
        return await _contexto.Pesquisas
            .Include(p => p.GruposQuestoes)
                .ThenInclude(g => g.Questoes)
            .Include(p => p.Resultados)
                .ThenInclude(r => r.Respostas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pesquisa>> ObterPesquisasComResultadosAsync()
    {
        return await _contexto.Pesquisas
            .Include(p => p.GruposQuestoes)
                .ThenInclude(g => g.Questoes)
            .Include(p => p.Resultados)
            .ToListAsync();
    }

    public async Task<ResultadoPessoa> AdicionarResultadoAsync(Guid pesquisaId, ResultadoPessoa resultado)
    {
        var pesquisa = await ObterPorIdAsync(pesquisaId);
        if (pesquisa == null)
            throw new ArgumentException("Pesquisa não encontrada", nameof(pesquisaId));

        // Verificar se a pesquisa existe no contexto com navegação
        var pesquisaCompleta = await _contexto.Pesquisas
            .Include(p => p.Resultados)
            .FirstOrDefaultAsync(p => p.Id == pesquisaId);

        if (pesquisaCompleta == null)
            throw new ArgumentException("Pesquisa não encontrada", nameof(pesquisaId));

        var resultadoExistente = pesquisaCompleta.Resultados
            .FirstOrDefault(r => r.Id == resultado.Id);

        if (resultadoExistente != null)
        {
            // Atualizar resultado existente
            resultadoExistente.NomePessoa = resultado.NomePessoa;
            resultadoExistente.Email = resultado.Email;
            resultadoExistente.DataAvaliacao = resultado.DataAvaliacao;
            resultadoExistente.Respostas = resultado.Respostas;
        }
        else
        {
            // Adicionar novo resultado
            pesquisaCompleta.Resultados.Add(resultado);
        }

        await _contexto.SaveChangesAsync();
        return resultado;
    }

    public async Task<IEnumerable<ResultadoPessoa>> ObterResultadosPesquisaAsync(Guid pesquisaId)
    {
        var pesquisa = await _contexto.Pesquisas
            .Include(p => p.Resultados)
                .ThenInclude(r => r.Respostas)
            .FirstOrDefaultAsync(p => p.Id == pesquisaId);

        return pesquisa?.Resultados ?? new List<ResultadoPessoa>();
    }

    public async Task<ResultadoPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId)
    {
        var pesquisa = await _contexto.Pesquisas
            .Include(p => p.Resultados)
                .ThenInclude(r => r.Respostas)
            .FirstOrDefaultAsync(p => p.Id == pesquisaId);

        return pesquisa?.Resultados.FirstOrDefault(r => r.Id == pessoaId);
    }

    public override async Task<IEnumerable<Pesquisa>> ObterTodosAsync()
    {
        return await _contexto.Pesquisas
            .Include(p => p.GruposQuestoes)
                .ThenInclude(g => g.Questoes)
            .Include(p => p.Resultados)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();
    }

    public override async Task<Pesquisa?> ObterPorIdAsync(Guid id)
    {
        return await ObterPesquisaCompletaAsync(id);
    }
}