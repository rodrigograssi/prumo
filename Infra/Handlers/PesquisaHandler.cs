using PesquisaX.Core.Interfaces;
using PesquisaX.Core.Modelos;
using PesquisaX.Core.Repositorios;
using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.DTO;

namespace PesquisaX.Infra.Handlers;

public class PesquisaHandler(IPesquisaRepositorio repo): IPesquisaHandler
{
    public async Task<RetornoPadrao> Criar(PesquisaCrud comando)
    {
        var pesquisa = new Pesquisa(comando.Nome);
        await repo.GravarAsync(pesquisa);
        return new RetornoPadrao(true, "Pesquisa criada com sucesso");
    }

    public async Task<RetornoPadrao> Alterar(PesquisaCrud comando)
    {
        Guid id;
        if (!Guid.TryParse(comando.Id, out id))
            return new RetornoPadrao("Erro ao alterar a pequisa", "Guid invalido");

        var pesquisa = await repo.SelecionaAsync(id);
        if(pesquisa == null)
            return new RetornoPadrao("Erro ao alterar a pequisa", "Pesquisa não localizada");
        
        pesquisa.Alterar(comando.Nome);
        await repo.GravarAsync(pesquisa);
        return new RetornoPadrao(true, "Pesquisa alterada com sucesso");
    }

    public async Task<RetornoPadrao> Excluir(Guid pesquisaId)
    {
        var pesquisa = await repo.SelecionaAsync(pesquisaId);
        if(pesquisa == null)
            return new RetornoPadrao("Erro ao excluir a pequisa", "Pesquisa não localizada");
        
        await repo.ExcluirAsync(pesquisa);
        return new RetornoPadrao(true, "Pesquisa excluída com sucesso");
    }
    
    public async Task<RetornoPadrao> CriarGrupoDeQuestao(GrupoQuestaoCrud comando)
    {
        var pesquisa = await repo.SelecionaAsync(Guid.Parse(comando.PesquisaId));
        if(pesquisa == null)
            return new RetornoPadrao("Erro ao excluir a pequisa", "Pesquisa não localizada");
        
        var grupo = new GrupoQuestao(comando.Nome, comando.Categoria, comando.Cor);
        pesquisa.AdicionarGrupoQuestao(grupo);
        
        await repo.GravarAsync(pesquisa);
        return new RetornoPadrao(true, "Grupo criado com sucesso");
    }
    
    public async Task<RetornoPadrao> AlterarGrupoDeQuestao(GrupoQuestaoCrud comando)
    {
        var grupo = await repo.SelecionaGrupoQuestaoAsync(Guid.Parse(comando.Id));
        if(grupo == null)
            return new RetornoPadrao( "Erro ao alterar grupo de questão", "Grupo não localizada");;
        
        grupo.Alterar(comando.Nome, comando.Categoria, comando.Cor);
        await repo.GravarGrupoDeQuestaoAsync(grupo);
        return new RetornoPadrao(true, "Grupo alterado com sucesso");
    }
    
    public async Task<RetornoPadrao> ExcluirGrupoDeQuestao(GrupoQuestaoCrud comando)
    {
        var pesquisa = await repo.SelecionaAsync(Guid.Parse(comando.PesquisaId));
        if(pesquisa == null)
            return new RetornoPadrao( "Erro ao excluir grupo de questão", "Pequisa não localizada");
        
        var grupo = pesquisa.GruposQuestoes.FirstOrDefault(g => g.Id == Guid.Parse(comando.Id));
        if(grupo== null)
            return new RetornoPadrao( "Erro ao excluir grupo de questão", "Grupo não localizada");;
        
        await repo.ExcluirGrupoQuestoes(grupo);
        return new RetornoPadrao(true, "Grupo excluído com sucesso");
    }

    public async Task<RetornoPadrao> CriarQuestao(QuestaoCrud comando)
    {
        var grupoQuestao = await repo.SelecionaGrupoQuestaoAsync(Guid.Parse(comando.GrupoDeQuestaoId));
        if (grupoQuestao == null)
            return new RetornoPadrao( "Erro ao criar questão", "Questão não localizada");

        var questao = new Questao(comando.Texto, comando.EInvertida, 0);
        grupoQuestao.AdicionarQuestao(questao);
        
        await repo.GravarGrupoDeQuestaoAsync(grupoQuestao);
        return new RetornoPadrao(true, "Questão criada com sucesso");
    }
    
    public async Task<RetornoPadrao>  AlterarQuestao(QuestaoCrud comando)
    {
        var questao = await repo.SelecionaQuestaoAsync(Guid.Parse(comando.Id));
        if (questao == null)
            return new RetornoPadrao( "Erro ao alterar questão", "Questão não localizada");

        questao.Alterar(comando.Texto, comando.EInvertida, comando.Ordem);        
        
        await repo.GravarQuestaoAsync(questao);
        return new RetornoPadrao(true, "Questão alterada com sucesso");
    }
    
    public async Task<RetornoPadrao>  ExcluirQuestao(QuestaoCrud comando)
    {
        var questao = await repo.SelecionaQuestaoAsync(Guid.Parse(comando.Id));
        if (questao == null)
            return new RetornoPadrao( "Erro ao excluir questão", "Questão não localizada");;

        await repo.ExcluiQuestaoAsync(questao);
        return new RetornoPadrao(true, "Questão excluída com sucesso");
    }

    public async Task<RetornoPadrao> CriarResposta(RespostaPessoaCrud comando)
    {
        Guid pesquisaId;
        if(!Guid.TryParse(comando.PesquisaId, out pesquisaId))
            return new RetornoPadrao("Erro ao criar resposta", "Guid inválido");
        
        var respostaPessoa = new RespostaPessoa(pesquisaId, comando.NomePessoa, comando.Email);
        foreach (var item in comando.Respostas)
        {
            if(!Guid.TryParse(item.QuestaoId, out var questaoId))
                return new RetornoPadrao("Erro ao criar resposta", "Guid da questão inválido");
            
            var questao = new Resposta(questaoId, item.ValorBruto, item.ValorProcessado);
            respostaPessoa.AdicionarResposta(questao);
        }
        
        await repo.GravarRespostaPessoaAsync(respostaPessoa);
        return new RetornoPadrao(true,"Resposta criada com sucesso");
        
    }
}