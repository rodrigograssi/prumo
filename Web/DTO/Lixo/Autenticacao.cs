
using Prumo.Shared.Views;

namespace Prumo.Web.Dto
{
    public class Autenticacao
    {
        public UsuarioView Usuario { get; set; } = new();
        public string Token { get; set; } = string.Empty;
    }
}
