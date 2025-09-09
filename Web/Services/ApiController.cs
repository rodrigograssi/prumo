using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using PesquisaX.Shared.DTO;
using PesquisaX.Shared.Views;
using PesquisaX.Web.Dto;

namespace PesquisaX.Web.Services
{
    /// <summary>
    /// Controller genérico para comunicação com APIs REST
    /// Fornece métodos para operações CRUD com tratamento robusto de erros
    /// </summary>
    /// <typeparam name="T">Tipo de retorno esperado das operações</typeparam>
    /// <example>
    /// Uso:
    /// var resultado = await ApiController&lt;ClasseDeIpView&gt;.GetAsync("classes");
    /// var criacao = await ApiController&lt;ClasseDeIpView&gt;.GravarAsync(novaClasse, "classes");
    /// var atualizacao = await ApiController&lt;ClasseDeIpView&gt;.GravarAsync(classe, "classes", 1);
    /// </example>
    public class ApiController<T>
    {
        private readonly HttpClient _httpClient;

        public ApiController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        /// <summary>
        /// Grava dados usando POST (criação) ou PUT (atualização)
        /// </summary>
        /// <typeparam name="TData">Tipo dos dados a serem enviados</typeparam>
        /// <param name="dados">Dados para gravar</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <param name="id">ID para atualização (0 = criação)</param>
        /// <returns>Resultado da operação</returns>
        public async Task<RetornoPadrao<T>> GravarAsync<TData>(TData dados, string enderecoApi, int id = 0)
        {
            try
            {
                ConfigurarAutenticacao();

                var json = JsonSerializer.Serialize(dados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var url = ConstruirUrl(enderecoApi,0);
                
                var response = id > 0 
                    ? await _httpClient.PutAsync(url, content)
                    : await _httpClient.PostAsync(url, content);

                return await ProcessarResposta(response);
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao<T>($"Erro de rede: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new RetornoPadrao<T>( "Timeout na requisição");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao<T>($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<T>($"Erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Método obsoleto - use GravarAsync
        /// </summary>
        [Obsolete("Use GravarAsync<TData> para melhor type safety")]
        public async Task<RetornoPadrao<T>> Gravar(dynamic dados, string enderecoApi, int id = 0)
        {
            return await GravarAsync(dados, enderecoApi, id);
        }

        /// <summary>
        /// Método obsoleto - use GravarAsync
        /// </summary>
        [Obsolete("Use GravarAsync<TData> para melhor type safety")]
        public async Task<RetornoPadrao<T>> Gravar(T dados, string enderecoApi, int id = 0)
        {
            return await GravarAsync(dados, enderecoApi, id);
        }

        /// <summary>
        /// Executa uma requisição POST
        /// </summary>
        /// <typeparam name="TData">Tipo dos dados a serem enviados</typeparam>
        /// <param name="dadosEnviados">Dados para enviar</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <returns>Resultado da operação</returns>
        public async Task<RetornoPadrao<T>> PostAsync<TData>(TData dadosEnviados, string enderecoApi)
        {
            try
            {
                ConfigurarAutenticacao();

                var json = JsonSerializer.Serialize(dadosEnviados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"{enderecoApi}";

                var response = await _httpClient.PostAsync(url, content);
                return await ProcessarResposta(response);
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao<T>($"Erro de rede: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new RetornoPadrao<T>("Timeout na requisição");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao<T>($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<T>($"Erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Método obsoleto - use PostAsync
        /// </summary>
        [Obsolete("Use PostAsync<TData> para melhor type safety")]
        public async Task<RetornoPadrao<T>> Post(dynamic dadosEnviados, string enderecoApi)
        {
            return await PostAsync(dadosEnviados, enderecoApi);
        }


        /// <summary>
        /// Executa uma requisição GET
        /// </summary>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <returns>Resultado da operação</returns>
        public async Task<RetornoPadrao<T>> GetAsync(string enderecoApi)
        {
            try
            {
                ConfigurarAutenticacao();

                var url = $"{enderecoApi}";
                var response = await _httpClient.GetAsync(url);
                
                return await ProcessarResposta(response);
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao<T>($"Erro de rede: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new RetornoPadrao<T>("Timeout na requisição");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao<T>($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<T>($"Erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição DELETE
        /// </summary>
        /// <param name="id">ID do recurso a ser excluído</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <returns>Resultado da operação</returns>
        public async Task<RetornoPadrao<T>> DeleteAsync(int id, string enderecoApi)
        {
            try
            {
                ConfigurarAutenticacao();

                var url = $"{enderecoApi}/{id}";
                var response = await _httpClient.DeleteAsync(url);
                
                return await ProcessarResposta(response);
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao<T>($"Erro de rede: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new RetornoPadrao<T>("Timeout na requisição");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao<T>($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<T>($"Erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Método obsoleto - use GetAsync
        /// </summary>
        [Obsolete("Use GetAsync para melhor tratamento de erros")]
        public async Task<RetornoPadrao<T>> Get(string enderecoApi)
        {
            return await GetAsync(enderecoApi);
        }

        /// <summary>
        /// Método obsoleto - use DeleteAsync
        /// </summary>
        [Obsolete("Use DeleteAsync para melhor tratamento de erros")]
        public async Task<RetornoPadrao<T>> Delete(int id, string enderecoApi)
        {
            return await DeleteAsync(id, enderecoApi);
        }
        
        public async Task<RetornoPadrao<UsuarioView>> CarregarDadosDoUsuario(string token)
        {
            HttpResponseMessage response;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
            string enderecoApi = $"/api/auth/userinfo";
            response = await _httpClient.GetAsync(enderecoApi);
            try
            {
                var resultado = response.Content.ReadAsStringAsync().Result;
                var retorno = JsonSerializer.Deserialize<RetornoPadrao<UsuarioView>>(resultado, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return retorno;
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao<UsuarioView>($"Erro de rede: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<UsuarioView>($"Erro inesperado: {ex.Message}");
            }

        }


        #region Métodos Auxiliares Privados

        /// <summary>
        /// Configura a autenticação no HttpClient
        /// </summary>
        private void ConfigurarAutenticacao()
        {
            if (!string.IsNullOrEmpty(ConfiguracoesWeb.Autenticacao?.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", ConfiguracoesWeb.Autenticacao.Token);
            }
        }

        /// <summary>
        /// Constrói a URL completa baseada no endpoint e ID
        /// </summary>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <param name="id">ID opcional para operações de atualização</param>
        /// <returns>URL completa</returns>
        private string ConstruirUrl(string enderecoApi, int id = 0)
        {
            var baseUrl = $"{enderecoApi}";
            return id > 0 ? $"{baseUrl}/{id}" : baseUrl;
        }

        /// <summary>
        /// Processa a resposta HTTP e converte para RetornoPadrao
        /// </summary>
        /// <param name="response">Resposta HTTP</param>
        /// <returns>Resultado processado</returns>
        private async Task<RetornoPadrao<T>> ProcessarResposta(HttpResponseMessage response)
        {
            var resultado = await response.Content.ReadAsStringAsync();

            // Tratar resposta vazia
            if (string.IsNullOrEmpty(resultado))
            {
                return new RetornoPadrao<T>("Resposta vazia do servidor");
            }

            // Tratar códigos de status HTTP específicos
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new RetornoPadrao<T>("Token de autenticação inválido ou expirado");
                
                case HttpStatusCode.Forbidden:
                    return new RetornoPadrao<T>("Acesso negado");
                
                case HttpStatusCode.NotFound:
                    return new RetornoPadrao<T>("Recurso não encontrado");
                
                case HttpStatusCode.BadRequest:
                    var retornoBadRequest = JsonSerializer.Deserialize<RetornoPadrao<T>>(resultado);
                    return retornoBadRequest ?? new RetornoPadrao<T>("Dados de entrada inválidos");
            }

            // Para códigos de erro gerais
            if (!response.IsSuccessStatusCode)
            {
                return new RetornoPadrao<T>("Erro HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
            }

            // Deserializar resposta de sucesso
            try
            {
                var retorno = JsonSerializer.Deserialize<RetornoPadrao<T>>(resultado);
                return retorno ?? new RetornoPadrao<T>("Erro ao processar resposta do servidor");
            }
            catch (JsonException)
            {
                // Se falhar a deserialização, retornar erro de formato
                return new RetornoPadrao<T>("Formato de resposta inválido do servidor");
            }
        }

        #endregion
    }
}
