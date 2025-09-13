using Microsoft.EntityFrameworkCore;
using PesquisaX.Core.Modelos;
using PesquisaX.Core.Repositorios;
using PesquisaX.Infra.Contexto;
using PesquisaX.Shared.Views;

namespace PesquisaX.Infra.Repositorios;

public class PesquisaRepositorio(PesquisaXContexto db): IPesquisaRepositorio
{
    public async Task AlterarAsync(Pesquisa pesquisa)
    {
        db.Pesquisas.Update(pesquisa);
        await db.SaveChangesAsync();
    }
    
    public async Task CriarAsync(Pesquisa pesquisa)
    {
        await db.Pesquisas.AddAsync(pesquisa);
        await db.SaveChangesAsync();
    }

    public async Task<Pesquisa?> SelecionaAsync(Guid id)
    {
        var pesquisa = await db.Pesquisas
            .Include(p => p.RespostasPessoa)
            .Include(p => p.GruposQuestoes)
                .ThenInclude( g => g.Questoes)
            .FirstOrDefaultAsync(p => p.PublicId == id);
        return pesquisa;
    }

    public async Task<Pesquisa?> SelecionaAsync(int id)
    {
        var pesquisa = await db.Pesquisas
            .Include(p => p.RespostasPessoa)
            .Include(p => p.GruposQuestoes)
            .ThenInclude( g => g.Questoes)
            .FirstOrDefaultAsync(p => p.Id == id);
        
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
        var grupo = await db.GruposQuestoes.FirstOrDefaultAsync(g => g.PublicId == id);
        return grupo;
    }

    public async Task<GrupoQuestao?> SelecionaGrupoQuestaoAsync(int id)
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
        var grupo = await db.Questoes.FirstOrDefaultAsync(q => q.PublicId == parse);
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

    
    public async Task<IEnumerable<RespostaPessoa>> ObterResultadosPesquisaAsync(int pesquisaId)
    {
        var resultados = await db.ResultadosPessoas
            .Include(r => r.Respostas)
            .Where(r => r.PesquisaId == pesquisaId)
            .ToListAsync();
     
        return resultados;
    }

    public async Task<RespostaPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId)
    {
        var retorno = await db.ResultadosPessoas
            .Include(r => r.Respostas)
            .FirstOrDefaultAsync( r => r.PublicId == pessoaId);

        return retorno;
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
                Id = p.Id,
                PublicId = p.PublicId,
                Nome = p.Nome,
                DataCriacao = p.DataCriacao,
                TotalDeGrupos = p.GruposQuestoes.Count(),
                TotalDePerguntas = p.GruposQuestoes.SelectMany(g => g.Questoes).Count(),
                TotalDeRespostas = p.RespostasPessoa.Count()
            })
            .ToListAsync();

        return pesquisas;
    }

    public async Task<int> ObtemPesquisaId(Guid pesquisaId)
    {
        return await db.Pesquisas.Where(p => p.PublicId == pesquisaId).Select(p => p.Id).FirstOrDefaultAsync();
    }

    public async Task<List<GrupoQuestao>> ObtemGruposDePergunta(int pesquisaId)
    {
        var grupos = await db.GruposQuestoes
            .Where(g => g.PesquisaId == pesquisaId)
            .Include(g => g.Questoes)
            .ToListAsync();
        
        return grupos;
    }

    public async Task<List<MediaDeResultadosDosGruposView>> ObtemMediaDosResultadosPorGrupoView(Guid personId)
    {
        // 1. Buscar a pessoa específica com suas respostas
        var respostaPessoa = await db.ResultadosPessoas
            .Include(rp => rp.Respostas)
            .FirstOrDefaultAsync(rp => rp.PublicId == personId);

        if (respostaPessoa == null)
            return new List<MediaDeResultadosDosGruposView>();

        // 2. Buscar questões e grupos relacionados às respostas da pessoa
        var questoesIds = respostaPessoa.Respostas.Select(r => r.QuestaoId).ToList();
        
        var questoes = await db.Questoes
            .Where(q => questoesIds.Contains(q.Id))
            .ToListAsync();

        var gruposIds = questoes.Select(q => q.GrupoDeQuestaoId).Distinct().ToList();
        
        var grupos = await db.GruposQuestoes
            .Where(g => gruposIds.Contains(g.Id))
            .ToListAsync();

        // 3. Agrupar respostas por grupo e calcular percentuais
        var resultado = new List<MediaDeResultadosDosGruposView>();

        foreach (var grupo in grupos)
        {
            // Buscar questões deste grupo
            var questoesDoGrupo = questoes.Where(q => q.GrupoDeQuestaoId == grupo.Id).ToList();
            
            // Buscar respostas para questões deste grupo
            var respostasDoGrupo = respostaPessoa.Respostas
                .Where(r => questoesDoGrupo.Any(q => q.Id == r.QuestaoId))
                .Select(r => r.ValorBruto)
                .ToList();

            if (respostasDoGrupo.Any())
            {
                resultado.Add(new MediaDeResultadosDosGruposView
                {
                    Categoria = grupo.Categoria,
                    Cor = grupo.Cor,
                    Valor = grupo.CalcularPercentual(respostasDoGrupo)
                });
            }
        }

        return resultado;
    }

    public async Task<List<Questao>> ObterQuestoes(Guid pesquisaId)
    {
        var id = await ObtemPesquisaId(pesquisaId);
        var questoes = await db.GruposQuestoes
                            .Include(q => q.Questoes)
                            .Where(g => g.PesquisaId == id)
                            .SelectMany( g => g.Questoes)
                            .ToListAsync();
        return questoes;
    }
}