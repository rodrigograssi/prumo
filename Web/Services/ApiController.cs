using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using Prumo.Shared.DTO;
using Prumo.Shared.Views;
using Prumo.Web.Dto;

namespace Prumo.Web.Services
{
    /// <summary>
    /// Controller base para comunicação com APIs REST (sem tipo genérico)
    /// Fornece métodos para operações que retornam apenas RetornoPadrao
    /// </summary>
    /// <example>
    /// Uso:
    /// var resultado = await apiController.PostAsync(dados, "api/endpoint");
    /// var exclusao = await apiController.DeleteAsync("123", "api/endpoint");
    /// </example>
    public class ApiController
    {
        protected readonly HttpClient _httpClient;

        public ApiController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Executa uma requisição POST que retorna apenas RetornoPadrao
        /// </summary>
        /// <typeparam name="TData">Tipo dos dados a serem enviados</typeparam>
        /// <param name="dadosEnviados">Dados para enviar</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> PostAsync<TData>(TData dadosEnviados, string enderecoApi)
        {
            try
            {
                var json = JsonSerializer.Serialize(dadosEnviados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(enderecoApi, content);
                return await ProcessarRespostaSemTipo(response);
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao($"Erro de rede: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new RetornoPadrao("Timeout na requisição");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição PUT que retorna apenas RetornoPadrao
        /// </summary>
        /// <typeparam name="TData">Tipo dos dados a serem enviados</typeparam>
        /// <param name="dadosEnviados">Dados para enviar</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <param name="id">ID do recurso</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> PutAsync<TData>(TData dadosEnviados, string enderecoApi, string id)
        {
            try
            {

                var json = JsonSerializer.Serialize(dadosEnviados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"{enderecoApi}/{id}";

                var response = await _httpClient.PutAsync(url, content);
                return await ProcessarRespostaSemTipo(response);
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao($"Erro de rede: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new RetornoPadrao("Timeout na requisição");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Grava dados usando POST (criação) ou PUT (atualização) que retorna apenas RetornoPadrao
        /// </summary>
        /// <typeparam name="TData">Tipo dos dados a serem enviados</typeparam>
        /// <param name="dados">Dados para gravar</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <param name="id">ID para atualização (null ou vazio = criação)</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> GravarAsync<TData>(TData dados, string enderecoApi, string? id = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(dados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var url = !string.IsNullOrEmpty(id) ? $"{enderecoApi}/{id}" : enderecoApi;
                
                var response = !string.IsNullOrEmpty(id)
                    ? await _httpClient.PutAsync(url, content)
                    : await _httpClient.PostAsync(url, content);

                return await ProcessarRespostaSemTipo(response);
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao($"Erro de rede: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new RetornoPadrao("Timeout na requisição");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição GET que retorna apenas RetornoPadrao
        /// </summary>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> GetAsync(string enderecoApi)
        {
            try
            {
                var response = await _httpClient.GetAsync(enderecoApi);
                return await ProcessarRespostaSemTipo(response);
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao($"Erro de rede: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new RetornoPadrao("Timeout na requisição");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição DELETE que retorna apenas RetornoPadrao
        /// </summary>
        /// <param name="id">ID do recurso a ser excluído</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> DeleteAsync(string id, string enderecoApi)
        {
            try
            {

                var url = $"{enderecoApi}/{id}";
                var response = await _httpClient.DeleteAsync(url);
                
                return await ProcessarRespostaSemTipo(response);
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao($"Erro de rede: {ex.Message}");
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return new RetornoPadrao("Timeout na requisição");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro inesperado: {ex.Message}");
            }
        }

        #region Métodos Auxiliares Protegidos


        /// <summary>
        /// Processa a resposta HTTP e converte para RetornoPadrao sem tipo
        /// </summary>
        /// <param name="response">Resposta HTTP</param>
        /// <returns>Resultado processado sem dados tipados</returns>
        protected async Task<RetornoPadrao> ProcessarRespostaSemTipo(HttpResponseMessage response)
        {
            var resultado = await response.Content.ReadAsStringAsync();

            // Tratar resposta vazia
            if (string.IsNullOrEmpty(resultado))
            {
                return new RetornoPadrao("Resposta vazia do servidor");
            }

            // Tratar códigos de status HTTP específicos
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new RetornoPadrao("Token de autenticação inválido ou expirado");
                
                case HttpStatusCode.Forbidden:
                    return new RetornoPadrao("Acesso negado");
                
                case HttpStatusCode.NotFound:
                    return new RetornoPadrao("Recurso não encontrado");
                
                case HttpStatusCode.BadRequest:
                    var retornoBadRequest = JsonSerializer.Deserialize<RetornoPadrao>(resultado);
                    return retornoBadRequest ?? new RetornoPadrao("Dados de entrada inválidos");
            }

            // Para códigos de erro gerais
            if (!response.IsSuccessStatusCode)
            {
                return new RetornoPadrao($"Erro HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
            }

            // Deserializar resposta de sucesso
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = null // Não forçar camelCase na serialização
                };
                
                var retorno = JsonSerializer.Deserialize<RetornoPadrao>(resultado, options);
                return retorno ?? new RetornoPadrao("Erro ao processar resposta do servidor");
            }
            catch (JsonException ex)
            {
                // Log do erro para debug
                Console.WriteLine($"[DEBUG] Erro de deserialização: {ex.Message}");
                Console.WriteLine($"[DEBUG] JSON recebido: {resultado}");
                
                // Se falhar a deserialização, retornar erro de formato
                return new RetornoPadrao("Formato de resposta inválido do servidor");
            }
        }

        #endregion
    }

    /// <summary>
    /// Controller genérico para comunicação com APIs REST (com tipo genérico)
    /// Fornece métodos para operações CRUD com tratamento robusto de erros e dados tipados
    /// </summary>
    /// <typeparam name="T">Tipo de retorno esperado das operações</typeparam>
    /// <example>
    /// Uso:
    /// var resultado = await ApiController&lt;UsuarioView&gt;.GetAsync("api/usuarios");
    /// var criacao = await ApiController&lt;UsuarioView&gt;.PostAsync(novoUsuario, "api/usuarios");
    /// </example>
    public class ApiController<T> : ApiController
    {
        public ApiController(HttpClient httpClient) : base(httpClient)
        {
        }
        /// <summary>
        /// Grava dados usando POST (criação) ou PUT (atualização)
        /// </summary>
        /// <typeparam name="TData">Tipo dos dados a serem enviados</typeparam>
        /// <param name="dados">Dados para gravar</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <param name="id">ID para atualização (null ou vazio = criação)</param>
        /// <returns>Resultado da operação</returns>
        public async Task<RetornoPadrao<T>> GravarAsync<TData>(TData dados, string enderecoApi, string? id = null)
        {
            try
            {

                var json = JsonSerializer.Serialize(dados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var url = !string.IsNullOrEmpty(id) ? $"{enderecoApi}/{id}" : enderecoApi;
                
                var response = !string.IsNullOrEmpty(id)
                    ? await _httpClient.PutAsync(url, content)
                    : await _httpClient.PostAsync(url, content);

                return await ProcessarRespostaComTipo(response);
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
        /// Executa uma requisição POST que retorna dados tipados
        /// </summary>
        /// <typeparam name="TData">Tipo dos dados a serem enviados</typeparam>
        /// <param name="dadosEnviados">Dados para enviar</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <returns>Resultado da operação com dados tipados</returns>
        public new async Task<RetornoPadrao<T>> PostAsync<TData>(TData dadosEnviados, string enderecoApi)
        {
            try
            {

                var json = JsonSerializer.Serialize(dadosEnviados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(enderecoApi, content);
                return await ProcessarRespostaComTipo(response);
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
        /// Executa uma requisição GET
        /// </summary>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <returns>Resultado da operação</returns>
        public async Task<RetornoPadrao<T>> GetAsync(string enderecoApi)
        {
            try
            {

                var url = $"{enderecoApi}";
                var response = await _httpClient.GetAsync(url);
                
                return await ProcessarRespostaComTipo(response);
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
        /// Executa uma requisição DELETE que retorna dados tipados
        /// </summary>
        /// <param name="id">ID do recurso a ser excluído</param>
        /// <param name="enderecoApi">Endpoint da API</param>
        /// <returns>Resultado da operação com dados tipados</returns>
        public new async Task<RetornoPadrao<T>> DeleteAsync(string id, string enderecoApi)
        {
            try
            {

                var url = $"{enderecoApi}/{id}";
                var response = await _httpClient.DeleteAsync(url);
                
                return await ProcessarRespostaComTipo(response);
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

        
        public async Task<RetornoPadrao<UsuarioView>> CarregarDadosDoUsuario()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/auth/userinfo");
                var resultado = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrEmpty(resultado))
                {
                    return new RetornoPadrao<UsuarioView>("Resposta vazia do servidor");
                }

                var retorno = JsonSerializer.Deserialize<RetornoPadrao<UsuarioView>>(resultado, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return retorno ?? new RetornoPadrao<UsuarioView>("Erro ao processar resposta do servidor");
            }
            catch (HttpRequestException ex)
            {
                return new RetornoPadrao<UsuarioView>($"Erro de rede: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return new RetornoPadrao<UsuarioView>($"Erro ao processar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<UsuarioView>($"Erro inesperado: {ex.Message}");
            }
        }


        #region Métodos Auxiliares Privados



        /// <summary>
        /// Processa a resposta HTTP e converte para RetornoPadrao com tipo
        /// </summary>
        /// <param name="response">Resposta HTTP</param>
        /// <returns>Resultado processado com dados tipados</returns>
        private async Task<RetornoPadrao<T>> ProcessarRespostaComTipo(HttpResponseMessage response)
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
                return new RetornoPadrao<T>($"Erro HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
            }

            // Deserializar resposta de sucesso
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = null // Não forçar camelCase na serialização
                };
                
                var retorno = JsonSerializer.Deserialize<RetornoPadrao<T>>(resultado, options);
                return retorno ?? new RetornoPadrao<T>("Erro ao processar resposta do servidor");
            }
            catch (JsonException ex)
            {
                // Log do erro para debug
                Console.WriteLine($"[DEBUG] Erro de deserialização: {ex.Message}");
                Console.WriteLine($"[DEBUG] JSON recebido: {resultado}");
                
                // Se falhar a deserialização, retornar erro de formato
                return new RetornoPadrao<T>("Formato de resposta inválido do servidor");
            }
        }

        #endregion
    }
}
