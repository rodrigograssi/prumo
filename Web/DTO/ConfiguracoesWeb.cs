
using PesquisaX.Shared.Views;

namespace PesquisaX.Web.Dto
{
    public class ConfiguracoesWeb
    {

        public static Autenticacao? Autenticacao = new();
        public static UsuarioView? UsuarioLogado;

        public static string EnderecoApi = "http://localhost:5092";
        //public static string EnderecoApi = "https://www2.fmvz.unesp.br/ip-api";
        //public static string EnderecoApi = "https://ip.fmvz.unesp.br/api/v1";
        public static string VersaoDoSistema = "1.0.2";

    }
}
