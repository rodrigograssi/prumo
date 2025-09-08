using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using PesquisaX.Web.Dto;

namespace PesquisaX.Web.Services
{
    public class AutenticacaoController : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.Delay(1);
            
            if (ConfiguracoesWeb.UsuarioLogado == null || string.IsNullOrEmpty(ConfiguracoesWeb.UsuarioLogado?.Token))
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, ConfiguracoesWeb.UsuarioLogado.Nome));

        
            var usuario = new ClaimsIdentity(claims, "autenticacao");
            var claimsPrincipal = new ClaimsPrincipal(usuario);

            return new AuthenticationState(claimsPrincipal);
        }

        public void Carregar()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        


    }
}
