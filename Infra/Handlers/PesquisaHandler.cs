using PesquisaX.Core.Interfaces;
using PesquisaX.Core.Modelos;
using PesquisaX.Core.Repositorios;
using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.DTO;

namespace PesquisaX.Infra.Handlers;

public class PesquisaHandler(IPesquisaRepositorio repo): IPesquisaHandler
{
    public async Task<RetornoPadrao<Pesquisa>> Criar(PesquisaCrud comando)
    {
        var pesquisa = new Pesquisa(comando.Nome);

        foreach (var grupo in comando.GruposQuestoes)
        {
            var novoGrupo = new GrupoQuestao(grupo.Nome, grupo.Categoria, grupo.Cor);
            foreach (var questao in grupo.Questoes)
            {
                var novaQuestao = new Questao(questao.Texto, questao.EInvertida);
                novoGrupo.AdicionarQuestao(novaQuestao);
            }
            
            pesquisa.AdicionarGrupoQuestao(novoGrupo);
        }
        
        await repo.CriarAsync(pesquisa);
        return new RetornoPadrao<Pesquisa>(true, "Pesquisa criada com sucesso", pesquisa);
    }

    public async Task<RetornoPadrao> Alterar(PesquisaCrud comando)
    {

        var pesquisa = await repo.SelecionaAsync(comando.PublicId);
        if(pesquisa == null)
            return new RetornoPadrao("Erro ao alterar a pequisa", "Pesquisa não localizada");
        
        // Atualizar nome da pesquisa
        pesquisa.Alterar(comando.Nome);

        // Remover grupos que não existem mais no comando
        var idsGruposComando = new HashSet<int>(
            comando.GruposQuestoes.Select(g =>g.Id)
        );
        
        var gruposParaRemover = pesquisa.GruposQuestoes
            .Where(gq => !idsGruposComando.Contains(gq.Id))
            .ToList();

        foreach (var grupo in gruposParaRemover)
        {
            pesquisa.RemoveGrupo(grupo);
            await repo.ExcluirGrupoQuestoes(grupo); 
        }

        // Processar grupos do comando
        foreach (var grupoComando in comando.GruposQuestoes)
        {
            if (grupoComando.Id == 0)
            {
                // Novo grupo
                var novoGrupo = new GrupoQuestao(grupoComando.Nome, grupoComando.Categoria, grupoComando.Cor);
                foreach (var questao in grupoComando.Questoes)
                {
                    var novaQuestao = new Questao(questao.Texto, questao.EInvertida);
                    novoGrupo.AdicionarQuestao(novaQuestao);
                }
                pesquisa.AdicionarGrupoQuestao(novoGrupo);
            }
            else
            {
                var grupoExistente = pesquisa.GruposQuestoes.FirstOrDefault(g => g.Id == grupoComando.Id);
                if (grupoExistente != null)
                {
                    grupoExistente.Alterar(grupoComando.Nome, grupoComando.Categoria, grupoComando.Cor);

                    // Remover questões que não existem mais no comando
                    var idsQuestoesComando = new HashSet<int>(
                        grupoComando.Questoes
                            .Select(g => g.Id)
                    );
                    
                    var questoesParaRemover = grupoExistente.Questoes
                        .Where(geq => !idsQuestoesComando.Contains(geq.Id))
                        .ToList();

                    foreach (var questao in questoesParaRemover)
                    {
                        grupoExistente.RemoveQuestao(questao);
                        await repo.ExcluiQuestaoAsync(questao);
                    }

                    // Processar questões do comando
                    foreach (var questaoComando in grupoComando.Questoes)
                    {
                        if (questaoComando.Id == 0)
                        {
                            // Nova questão
                            var novaQuestao = new Questao(questaoComando.Texto, questaoComando.EInvertida);
                            grupoExistente.AdicionarQuestao(novaQuestao);
                        }
                        else
                        {
                            // Questão existente - atualizar
                            var questaoExistente = grupoExistente.Questoes.FirstOrDefault(q => q.Id == questaoComando.Id);
                            if (questaoExistente != null)
                            {
                                questaoExistente.Alterar(questaoComando.Texto, questaoComando.EInvertida, questaoComando.Ordem);
                            }
                        }
                    }
                }
            }
        }

        await repo.AlterarAsync(pesquisa);
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
        var pesquisa = await repo.SelecionaAsync(comando.PesquisaId);
        if(pesquisa == null)
            return new RetornoPadrao("Erro ao excluir a pequisa", "Pesquisa não localizada");
        
        var grupo = new GrupoQuestao(comando.Nome, comando.Categoria, comando.Cor);
        pesquisa.AdicionarGrupoQuestao(grupo);
        
        await repo.AlterarAsync(pesquisa);
        return new RetornoPadrao(true, "Grupo criado com sucesso");
    }
    
    public async Task<RetornoPadrao> AlterarGrupoDeQuestao(GrupoQuestaoCrud comando)
    {
        var grupo = await repo.SelecionaGrupoQuestaoAsync(comando.PublicId);
        if(grupo == null)
            return new RetornoPadrao( "Erro ao alterar grupo de questão", "Grupo não localizada");
        
        grupo.Alterar(comando.Nome, comando.Categoria, comando.Cor);
        await repo.GravarGrupoDeQuestaoAsync(grupo);
        return new RetornoPadrao(true, "Grupo alterado com sucesso");
    }
    
    public async Task<RetornoPadrao> ExcluirGrupoDeQuestao(GrupoQuestaoCrud comando)
    {
        var pesquisa = await repo.SelecionaAsync(comando.PesquisaId);
        if(pesquisa == null)
            return new RetornoPadrao( "Erro ao excluir grupo de questão", "Pequisa não localizada");
        
        var grupo = pesquisa.GruposQuestoes.FirstOrDefault(g => g.Id == comando.Id);
        if(grupo== null)
            return new RetornoPadrao( "Erro ao excluir grupo de questão", "Grupo não localizada");
        
        await repo.ExcluirGrupoQuestoes(grupo);
        return new RetornoPadrao(true, "Grupo excluído com sucesso");
    }

    public async Task<RetornoPadrao> CriarQuestao(QuestaoCrud comando)
    {
        var grupoQuestao = await repo.SelecionaGrupoQuestaoAsync(comando.GrupoDeQuestaoId);
        if (grupoQuestao == null)
            return new RetornoPadrao( "Erro ao criar questão", "Questão não localizada");

        var questao = new Questao(comando.Texto, comando.EInvertida);
        grupoQuestao.AdicionarQuestao(questao);
        
        await repo.GravarGrupoDeQuestaoAsync(grupoQuestao);
        return new RetornoPadrao(true, "Questão criada com sucesso");
    }
    
    public async Task<RetornoPadrao>  AlterarQuestao(QuestaoCrud comando)
    {
        var questao = await repo.SelecionaQuestaoAsync(comando.PublicId);
        if (questao == null)
            return new RetornoPadrao( "Erro ao alterar questão", "Questão não localizada");

        questao.Alterar(comando.Texto, comando.EInvertida, comando.Ordem);        
        
        await repo.GravarQuestaoAsync(questao);
        return new RetornoPadrao(true, "Questão alterada com sucesso");
    }
    
    public async Task<RetornoPadrao>  ExcluirQuestao(QuestaoCrud comando)
    {
        var questao = await repo.SelecionaQuestaoAsync(comando.PublicId);
        if (questao == null)
            return new RetornoPadrao( "Erro ao excluir questão", "Questão não localizada");

        await repo.ExcluiQuestaoAsync(questao);
        return new RetornoPadrao(true, "Questão excluída com sucesso");
    }

    public async Task<RetornoPadrao<RespostaPessoa>> CriarResposta(Guid pesquisaId, RespostaPessoaCrud comando)
    {
        var respostaPessoa = new RespostaPessoa(comando.PesquisaId, comando.NomePessoa, comando.Email);
        var questoes = await repo.ObterQuestoes(pesquisaId);
        
        foreach (var item in comando.Respostas)
        {
            var questao = questoes.FirstOrDefault(q => q.Id == item.QuestaoId);
            var resposta = new Resposta(questao, item.ValorBruto);
            respostaPessoa.AdicionarResposta(resposta);
        }
        await repo.GravarRespostaPessoaAsync(respostaPessoa);
        return new RetornoPadrao<RespostaPessoa>(true,"Resposta criada com sucesso", respostaPessoa);
        
    }

    public async Task<RetornoPadrao> ExcluirResultado(Guid pesquisaId, Guid personId)
    {
        var resultado = await repo.ObterResultadoPessoaAsync(pesquisaId, personId);
        if (resultado == null)
            return new RetornoPadrao("Resultado não localizado");

        await repo.ExcluirResultadoAsync(resultado);
        return new RetornoPadrao(true, "Resultado excluido com sucesso");
    }

}