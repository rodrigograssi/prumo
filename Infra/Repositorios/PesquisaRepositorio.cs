using Microsoft.EntityFrameworkCore;
using PesquisaX.Core.Modelos;
using PesquisaX.Core.Repositorios;
using PesquisaX.Infra.Contexto;
using PesquisaX.Shared.Views;

namespace PesquisaX.Infra.Repositorios;

public class PesquisaRepositorio(PesquisaXContexto db): IPesquisaRepositorio
{
    public async Task GravarAsync(Pesquisa pesquisa)
    {
        db.Pesquisas.Update(pesquisa);
        await db.SaveChangesAsync();
    }

    public async Task<Pesquisa?> SelecionaAsync(Guid id)
    {
        var pesquisa = await db.Pesquisas.FirstOrDefaultAsync(p => p.Id == id);
        return pesquisa;
    }

    public async Task ExcluirAsync(Pesquisa pesquisa)
    {
        db.Pesquisas.Remove(pesquisa);
        await db.SaveChangesAsync();
    }

    public async Task ExcluirGrupoQuestoes(GrupoQuestao grupo)
    {
        db.GruposQuestoes.Remove(grupo);
        await db.SaveChangesAsync();
    }

    public async Task<GrupoQuestao?> SelecionaGrupoQuestaoAsync(Guid id)
    {
        var grupo = await db.GruposQuestoes.FirstOrDefaultAsync(g => g.Id == id);
        return grupo;
    }

    public async Task GravarGrupoDeQuestaoAsync(GrupoQuestao grupo)
    {
        db.GruposQuestoes.Add(grupo);
        await db.SaveChangesAsync();
    }

    public async Task<Questao?> SelecionaQuestaoAsync(Guid parse)
    {
        var grupo = await db.Questoes.FirstOrDefaultAsync(q => q.Id == parse);
        return grupo;
    }

    public async Task GravarQuestaoAsync(Questao questao)
    {
        db.Questoes.Update(questao);
        await db.SaveChangesAsync();
    }

    public async Task ExcluiQuestaoAsync(Questao questao)
    {
        db.Questoes.Remove(questao);
        await db.SaveChangesAsync();
    }

    public async Task GravarRespostaPessoaAsync(RespostaPessoa respostaPessoa)
    {
        db.ResultadosPessoas.Update(respostaPessoa);
        await db.SaveChangesAsync();   
    }

    public Task<Pesquisa?> ObterPesquisaCompletaAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Pesquisa>> ObterPesquisasComResultadosAsync()
    {
        throw new NotImplementedException();
    }

    public Task<RespostaPessoa> AdicionarResultadoAsync(RespostaPessoa resposta)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<RespostaPessoa>> ObterResultadosPesquisaAsync(Guid pesquisaId)
    {
        var resultados = await db.ResultadosPessoas
            .Include(r => r.Respostas)
            .Where(r => r.PesquisaId == pesquisaId)
            .ToListAsync();
     
        return resultados;
    }

    public Task<RespostaPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Pesquisa>?> ObterTodasPequisaAsync()
    {
        var pequisas = await db.Pesquisas
            .Include(p => p.RespostasPessoa)
            .Include(p => p.GruposQuestoes)
                .ThenInclude( g => g.Questoes)
            .ToListAsync();
        return pequisas;
    }

    
    public async Task<List<PesquisaListaView>?> ObterPequisaListView()
    {
        var pesquisas = await db.Pesquisas
            .Select(p => new PesquisaListaView
            {
                Id = p.Id.ToString(),
                Nome = p.Nome,
                DataCriacao = p.DataCriacao,
                TotalDeGrupos = p.GruposQuestoes.Count(),
                TotalDePerguntas = p.GruposQuestoes.SelectMany(g => g.Questoes).Count(),
                TotalDeRespostas = p.RespostasPessoa.Count()
            })
            .ToListAsync();

        return pesquisas;
    }
    
}