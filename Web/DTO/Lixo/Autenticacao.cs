
using PesquisaX.Shared.Views;

namespace PesquisaX.Web.Dto
{
    public class Autenticacao
    {
        public UsuarioView Usuario { get; set; } = new();
        public string Token { get; set; } = string.Empty;
    }
}
