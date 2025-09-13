using PesquisaX.Core.Modelos;
using PesquisaX.Shared.Views;

namespace PesquisaX.Core.Extension;

public static class ConverteParaViewExtension
{
    public static UsuarioView ConverteParaView(this Usuario usuario)
    {
        var view = new UsuarioView()
        {
            Id = usuario.Id,
            PublicId = usuario.PublicId,
            Nome = usuario.Nome,
            Email = usuario.Email,
            DataCriacao = usuario.DataCriacao,
            Token = usuario.Sistema?.Token ?? string.Empty, 
        };
        return view;
    }


    public static RespostaPessoaView ConverteParaView(this RespostaPessoa respostaPessoa)
    {
        var view = new RespostaPessoaView()
        {
            Id = respostaPessoa.Id,
            PublicId = respostaPessoa.PublicId,
            PesquisaId = respostaPessoa.PesquisaId.ToString(),
            NomePessoa = respostaPessoa.NomePessoa,
            Email = respostaPessoa.Email,
            DataAvaliacao = respostaPessoa.DataAvaliacao,
        };
        
        return view;
    }

    public static RespostaPessoaView ConverteParaView(this RespostaPessoa respostaPessoa, List<GrupoQuestao> grupos)
    {
        var view = new RespostaPessoaView()
        {
            Id = respostaPessoa.Id,
            PublicId = respostaPessoa.PublicId,
            PesquisaId = respostaPessoa.PesquisaId.ToString(),
            NomePessoa = respostaPessoa.NomePessoa,
            Email = respostaPessoa.Email,
            DataAvaliacao = respostaPessoa.DataAvaliacao

        };
        var percentuais = respostaPessoa.ObterPercentuaisGrupos(grupos);
        view.PercentuaisGrupos = percentuais;
        view.MediaGeral = percentuais.Values.Average();

        return view;
    }
}