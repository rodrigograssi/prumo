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
            .Include(p => p.ResultadosPessoas)
                .ThenInclude(r => r.Respostas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pesquisa>> ObterPesquisasComResultadosAsync()
    {
        return await _contexto.Pesquisas
            .Include(p => p.GruposQuestoes)
                .ThenInclude(g => g.Questoes)
            .Include(p => p.ResultadosPessoas)
            .ToListAsync();
    }

    public async Task<ResultadoPessoa> AdicionarResultadoAsync(ResultadoPessoa resultado)
    {
        // var pesquisa = await ObterPorIdAsync(resultado.PesquisaId);
        // if (pesquisa == null)
        //     throw new ArgumentException("Pesquisa não encontrada");

        // Verifica se já existe
        // var resultadoExistente = pesquisa.ResultadosPessoas
        //     .FirstOrDefault(r => r.Id == resultado.Id);

        // if (resultadoExistente != null)
        // {
        //     // Atualiza dados básicos
        //     resultadoExistente.NomePessoa = resultado.NomePessoa;
        //     resultadoExistente.Email = resultado.Email;
        //     resultadoExistente.DataAvaliacao = resultado.DataAvaliacao;
        //
        //     // Remove as respostas antigas
        //     _contexto.Respostas.RemoveRange(resultadoExistente.Respostas);
        //
        //     // Adiciona as novas respostas já vinculadas ao resultado
        //     resultadoExistente.Respostas = resultado.Respostas
        //         .Select(r => new Resposta
        //         {
        //             QuestaoId = r.QuestaoId,
        //             ValorBruto = r.ValorBruto,
        //             ValorProcessado= r.ValorProcessado,
        //             ResultadoPessoaId = resultadoExistente.Id
        //         })
        //         .ToList();
        // }
        // else
        // {
            // Novo resultado já com respostas configuradas
            await _contexto.ResultadosPessoas.AddAsync(resultado);
            await _contexto.SaveChangesAsync();
            return resultado;
        //}

    }

    public async Task<IEnumerable<ResultadoPessoa>> ObterResultadosPesquisaAsync(Guid pesquisaId)
    {
        var pesquisa = await _contexto.Pesquisas
            .Include(p => p.ResultadosPessoas)
                .ThenInclude(r => r.Respostas)
            .FirstOrDefaultAsync(p => p.Id == pesquisaId);

        return pesquisa?.ResultadosPessoas ?? new List<ResultadoPessoa>();
    }

    public async Task<ResultadoPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId)
    {
        var pesquisa = await _contexto.Pesquisas
            .Include(p => p.ResultadosPessoas)
                .ThenInclude(r => r.Respostas)
            .FirstOrDefaultAsync(p => p.Id == pesquisaId);

        return pesquisa?.ResultadosPessoas.FirstOrDefault(r => r.Id == pessoaId);
    }

    public override async Task<IEnumerable<Pesquisa>> ObterTodosAsync()
    {
        return await _contexto.Pesquisas
            .Include(p => p.GruposQuestoes)
                .ThenInclude(g => g.Questoes)
            .Include(p => p.ResultadosPessoas)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync();
    }

    public override async Task<Pesquisa?> ObterPorIdAsync(Guid id)
    {
        return await ObterPesquisaCompletaAsync(id);
    }

    public override async Task AtualizarAsync(Pesquisa entidade)
    {
        _contexto.Pesquisas.Update(entidade);
        await _contexto.SaveChangesAsync();
    }
    
    
    
}