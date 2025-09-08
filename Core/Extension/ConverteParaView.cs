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
            Senha = usuario.Senha,
            DataCriacao = usuario.DataCriacao
        };
        return view;
    }
}