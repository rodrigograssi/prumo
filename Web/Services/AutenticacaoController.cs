using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Prumo.Web.Dto;
using ControleDeIp.FrontEnd.Services;

namespace Prumo.Web.Services
{
    public class AutenticacaoController(ApiUsuarioController apiUsuario) : AuthenticationStateProvider
    {

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Verificar se já tem utilizador em memória
            if (ConfiguracoesWeb.UsuarioLogado != null && !string.IsNullOrEmpty(ConfiguracoesWeb.Token))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, ConfiguracoesWeb.UsuarioLogado.Nome),
                    new Claim(ClaimTypes.Email, ConfiguracoesWeb.UsuarioLogado.Email),
                    new Claim(ClaimTypes.NameIdentifier, ConfiguracoesWeb.UsuarioLogado.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, "autenticacao");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }

            // Verificar se tem token no localStorage
            var token = await apiUsuario.ObterTokenDoLocalStorageAsync();
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    ConfiguracoesWeb.Token = token;
                    var resultado = await apiUsuario.LoginComToken(token);
                    
                    if (resultado.Sucesso && resultado.Data != null)
                    {
                        ConfiguracoesWeb.UsuarioLogado = resultado.Data;
                        ConfiguracoesWeb.Token = resultado.Data.Token;
                        
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, resultado.Data.Nome),
                            new Claim(ClaimTypes.Email, resultado.Data.Email),
                            new Claim(ClaimTypes.NameIdentifier, resultado.Data.PublicId.ToString())
                        };

                        var identity = new ClaimsIdentity(claims, "autenticacao");
                        return new AuthenticationState(new ClaimsPrincipal(identity));
                    }
                }
                catch
                {
                    // Token inválido, limpar
                    await apiUsuario.LogoutAsync();
                }
            }
            
            // Não autenticado -- Força limpar o cache
            await apiUsuario.LogoutAsync();
            return new AuthenticationState(new ClaimsPrincipal());
        }

        public void Carregar()
        {
            NotifyAuthenticationStateChanged( GetAuthenticationStateAsync());
        }

        


    }
}
