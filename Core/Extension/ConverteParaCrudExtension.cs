using PesquisaX.Core.Modelos;
using PesquisaX.Shared.Comandos;

namespace PesquisaX.Core.Extension;

public static class ConverteParaCrudExtension
{
    public static PesquisaCrud ConverteParaCrud(this Pesquisa pesquisa)
    {
        
        var view = new PesquisaCrud()
        {
            Id = pesquisa.Id,
            PublicId = pesquisa.PublicId,
            Nome = pesquisa.Nome,
            NomeResultadoDesejado = pesquisa.NomeDoResultadoDesejado,
            DataCriacao = pesquisa.DataCriacao,
            GruposQuestoes = pesquisa.GruposQuestoes.Select(g => g.ConverteParaCrud()).ToList(),
            RespostasPessoa = pesquisa.RespostasPessoa.Select(g => g.ConverteParaCrud()).ToList()
        };
        return view;
    }

    public static GrupoQuestaoCrud ConverteParaCrud(this GrupoQuestao grupo)
    {
        var view = new GrupoQuestaoCrud()
        {
            Id = grupo.Id,
            PublicId = grupo.PublicId,
            Nome = grupo.Nome,
            Categoria = grupo.Categoria,
            Cor = grupo.Cor,
            Ordem = grupo.Ordem,
            PesquisaId = grupo.PesquisaId,
            Questoes = grupo.Questoes.Select(q => q.ConverteParaCrud()).ToList(),
        };
        
        return view;
    }
    
    public static QuestaoCrud ConverteParaCrud(this Questao questao)
    {
        var view = new QuestaoCrud()
        {
            Id = questao.Id,
            PublicId = questao.PublicId,
            GrupoDeQuestaoId = questao.GrupoDeQuestaoId,
            Texto = questao.Texto,
            EInvertida = questao.EInvertida,
            Ordem = questao.Ordem,
        };
        return view;
    }
    
    public static RespostaCrud ConverteParaCrud(this Resposta resposta)
    {
        var view = new RespostaCrud()
        {
            Id = resposta.Id,
            PublicId = resposta.PublicId,
            ResultadoPessoaId = resposta.ResultadoPessoaId,
            QuestaoId = resposta.QuestaoId,
            ValorBruto = resposta.ValorBruto,
            ValorProcessado = resposta.ValorProcessado,
        };
        
        return view;
    }

    public static RespostaPessoaCrud ConverteParaCrud(this RespostaPessoa resposta)
    {
        var view = new RespostaPessoaCrud()
        {
            Id = resposta.Id,
            PublicId = resposta.PublicId,
            PesquisaId = resposta.PesquisaId,
            DataAvaliacao = resposta.DataAvaliacao,
            Email = resposta.Email,
            NomePessoa = resposta.NomePessoa,
            Respostas = resposta.Respostas.Select(r => r.ConverteParaCrud()).ToList(),
        };
        return view;
    }
}