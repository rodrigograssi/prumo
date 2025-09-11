using Microsoft.JSInterop;
using PesquisaX.Shared.DTO;
using PesquisaX.Shared.DTO.Requestes;
using PesquisaX.Shared.Views;
using PesquisaX.Web.Dto;
using PesquisaX.Web.Services;

namespace ControleDeIp.FrontEnd.Services
{
    public class ApiUsuarioController(ApiController<UsuarioView> client,ILogger<ApiUsuarioController> logger, IJSRuntime jsRuntime)
    {
        public async Task<RetornoPadrao<UsuarioView>?> Login(string email, string senha)
        {
            var comando = new LoginRequest()
            {
                Email = email,
                Senha = senha
            };

            var retorno = await client.PostAsync(comando, "api/Auth/login");
            return retorno;
        }
        
        public async Task<RetornoPadrao<UsuarioView>> LoginComToken(string token)
        {
            var retorno = await client.GetAsync($"api/Auth/login/token");
            return retorno;
        }
        
        public async Task LogoutAsync()
        {
            try
            {
                logger.LogInformation("Realizando logout");

                // Remover de ambos os storages
                await jsRuntime.InvokeVoidAsync("localStorage.removeItem", ConfiguracoesWeb.LocalStoragTokeneKey);;
                await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", ConfiguracoesWeb.LocalStoragTokeneKey);

                // Remover header de autorização
                ConfiguracoesWeb.UsuarioLogado = null;
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
                var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", ConfiguracoesWeb.LocalStoragTokeneKey);
                if (!string.IsNullOrEmpty(token))
                    return token;
                 
                return await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", ConfiguracoesWeb.LocalStoragTokeneKey);
            }
            catch
            {
                return null;
            }
        }
    }
}
