
using Prumo.Shared.Views;

namespace Prumo.Web.Dto
{
    public class ConfiguracoesWeb
    {
        public static UsuarioView? UsuarioLogado;
        public static string Token = string.Empty;
        
        public static string LocalStoragTokeneKey = "authToken";
        public static string LocalStorageChaveTema = "prumo-tema"; // "prumo-tema"

        public static string EnderecoApi = "http://localhost:5092";
        public static string VersaoDoSistema = "1.0.2";
    }
}
