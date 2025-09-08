using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PesquisaX.Shared.DTO;
using PesquisaX.Web.Dto;

namespace ControleDeIp.FrontEnd.Services
{
    public class ApiUsuarioController(HttpClient client)
    {
        public async Task<RetornoPadrao<Autenticacao>?> Autenticar(string email, string senha)
        {
            HttpResponseMessage response;

            string json = JsonSerializer.Serialize(new
            {
                Email = email,
                Senha = senha
            });

            StringContent content = new(json, Encoding.UTF8, "application/json");

            try
            {
                var path = $"{ConfiguracoesWeb.EnderecoApi}/auth/login";
                response = await client.PostAsync(path, content);

            }
            catch (Exception ex)
            {
                return new RetornoPadrao<Autenticacao>($"Erro: {ex.Message}");
            }
            var resultado = response.Content.ReadAsStringAsync().Result;
            var retorno = JsonSerializer.Deserialize<RetornoPadrao<Autenticacao>>(resultado);

            return retorno;
        }

        public async Task<RetornoPadrao<Autenticacao>?> VerificaToken(string token)
        {
            HttpResponseMessage response;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                response = await client.GetAsync($"{ConfiguracoesWeb.EnderecoApi}/auth/info-usuario");
                var resultado = response.Content.ReadAsStringAsync().Result;
                var retorno = JsonSerializer.Deserialize<RetornoPadrao<Autenticacao>>(resultado);
                return retorno;
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao<Autenticacao>($"Erro de rede: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<Autenticacao>($"Erro inesperado: {ex.Message}");
            }
        }
    }
}
