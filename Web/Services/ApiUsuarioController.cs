using System.IdentityModel.Tokens.Jwt;
using Microsoft.JSInterop;
using Prumo.Shared.DTO;
using Prumo.Shared.DTO.Requestes;
using Prumo.Shared.Views;
using Prumo.Web.Dto;
using Prumo.Web.Services;

namespace ControleDeIp.FrontEnd.Services
{
    public class ApiUsuarioController(AuthenticationHandler AuthController,
        ApiControllerGenerico client,
        ILogger<ApiUsuarioController> logger,
        IJSRuntime jsRuntime)
    {
        public async Task<RetornoPadrao<UsuarioView>?> Login(string email, string senha)
        {
            var comando = new LoginRequest()
            {
                Email = email,
                Senha = senha
            };

            var retorno = await client.PostAsync<UsuarioView>(comando, "api/Auth/login");
            return retorno;
        }

        public async Task<RetornoPadrao<UsuarioView>> LoginComToken(string token)
        {
            ConfiguracoesWeb.Token = token;
            
            var retorno = await client.GetAsync<UsuarioView>($"api/Auth/login/token");
            return retorno;
        }

        public async Task LogoutAsync()
        {
            try
            {
                logger.LogInformation("Realizando logout");

                // Remover de ambos os storages
                await jsRuntime.InvokeVoidAsync("localStorage.removeItem", ConfiguracoesWeb.LocalStoragTokeneKey);
                await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", ConfiguracoesWeb.LocalStoragTokeneKey);

                // Remover header de autorização
                ConfiguracoesWeb.UsuarioLogado = null;
                ConfiguracoesWeb.Token = string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao realizar logout");
            }
        }

        public async Task<string?> ObterTokenDoLocalStorageAsync()
        {
            try
            {
                // Tenta localStorage primeiro, depois sessionStorage
                var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem",
                    ConfiguracoesWeb.LocalStoragTokeneKey);
                if (!string.IsNullOrEmpty(token))
                    return token;

                return await jsRuntime.InvokeAsync<string>("sessionStorage.getItem",
                    ConfiguracoesWeb.LocalStoragTokeneKey);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> EstaAutenticadoAsync()
        {
            try
            {
                var token = await ObterTokenDoLocalStorageAsync();

                if (string.IsNullOrEmpty(token))
                    return false;

                // Verificar se o token não está expirado
                if (TokenEstaExpirado(token))
                {
                    await LogoutAsync();
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TokenEstaExpirado(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                return jwtToken.ValidTo <= DateTime.UtcNow;
            }
            catch
            {
                return true; // Se não conseguir ler o token, considera como expirado
            }
        }

        public async Task SalvarTokenAsync(string token, string storage = "localStorage")
        {
            await jsRuntime.InvokeVoidAsync($"{storage}.setItem", ConfiguracoesWeb.LocalStoragTokeneKey, token);
        }
    }
}
