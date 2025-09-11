using PesquisaX.Core.Modelos;
using PesquisaX.Shared.Views;

namespace PesquisaX.Core.Extension;

public static class ConverteParaViewExtension
{
    public static UsuarioView ConverteParaView(this Usuario usuario)
    {
        var view = new UsuarioView()
        {
            Id = usuario.Id.ToString(),
            Nome = usuario.Nome,
            Email = usuario.Email,
            DataCriacao = usuario.DataCriacao,
            Token = usuario.Sistema?.Token ?? string.Empty, 
        };
        return view;
    }


    public static RespostaPessoaView ConverteParaView(this RespostaPessoa respostaPessoa, List<GrupoQuestao> questoes)
    {
        var view = new RespostaPessoaView()
        {
            Id = respostaPessoa.Id.ToString(),
            PesquisaId = respostaPessoa.PesquisaId.ToString(),
            NomePessoa = respostaPessoa.NomePessoa,
            Email = respostaPessoa.Email,
            DataAvaliacao = respostaPessoa.DataAvaliacao,
            PercentuaisGrupos = respostaPessoa.ObterPercentuaisGrupos(questoes)
        };
        
        return view;
    }
}