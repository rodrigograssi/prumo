using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using Prumo.Shared.DTO;

namespace Prumo.Web.Services
{
    /// <summary>
    /// Controller unificado para comunicação com APIs REST
    /// Suporte a métodos com e sem retorno de dados tipados através de sobrecarga genérica
    /// </summary>
    /// <example>
    /// Uso:
    /// // Sem dados de retorno (apenas sucesso/erro)
    /// var resultado = await ApiController.PostAsync(dados, "api/usuarios");
    /// 
    /// // Com dados de retorno tipados
    /// var resultado = await ApiController.PostAsync<UsuarioView, UsuarioCrud>(dados, "api/usuarios");
    /// 
    /// // GET sempre retorna dados
    /// var resultado = await ApiController.GetAsync<List<UsuarioView>>("api/usuarios");
    /// </example>
    public class ApiControllerGenerico
    {
        private readonly HttpClient _httpClient;

        public ApiControllerGenerico(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region POST Methods

        /// <summary>
        /// Executa uma requisição POST que retorna dados tipados
        /// </summary>
        /// <typeparam name="TRetorno">Tipo de dados retornados</typeparam>
        /// <param name="dados">Dados para enviar</param>
        /// <param name="url">Endpoint da API</param>
        /// <returns>Resultado da operação com dados tipados</returns>
        public async Task<RetornoPadrao<TRetorno>> PostAsync<TRetorno>(object dados, string url)
        {
            try
            {
                var json = JsonSerializer.Serialize(dados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                return await ProcessarRespostaComTipo<TRetorno>(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<TRetorno>($"Erro na requisição: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição POST que retorna apenas sucesso/erro
        /// </summary>
        /// <param name="dados">Dados para enviar</param>
        /// <param name="url">Endpoint da API</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> PostAsync(object dados, string url)
        {
            try
            {
                var json = JsonSerializer.Serialize(dados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                return await ProcessarRespostaSemTipo(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro na requisição: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição POST de upload de arquivo
        /// </summary>
        /// <typeparam name="TRetorno">Tipo de dados retornados</typeparam>
        /// <param name="file">Arquivo para upload</param>
        /// <param name="url">Endpoint da API</param>
        /// <param name="fileParameterName">Nome do parâmetro do arquivo (padrão: "arquivo")</param>
        /// <returns>Resultado da operação com dados tipados</returns>
        public async Task<RetornoPadrao<TRetorno>> PostFileAsync<TRetorno>(IBrowserFile file, string url, string fileParameterName = "arquivo")
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB max
                using var streamContent = new StreamContent(fileStream);
                
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                content.Add(streamContent, fileParameterName, file.Name);

                var response = await _httpClient.PostAsync(url, content);
                return await ProcessarRespostaComTipo<TRetorno>(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<TRetorno>($"Erro no upload do arquivo: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição POST de upload de arquivo que retorna apenas sucesso/erro
        /// </summary>
        /// <param name="file">Arquivo para upload</param>
        /// <param name="url">Endpoint da API</param>
        /// <param name="fileParameterName">Nome do parâmetro do arquivo (padrão: "arquivo")</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> PostFileAsync(IBrowserFile file, string url, string fileParameterName = "arquivo")
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB max
                using var streamContent = new StreamContent(fileStream);
                
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                content.Add(streamContent, fileParameterName, file.Name);

                var response = await _httpClient.PostAsync(url, content);
                return await ProcessarRespostaSemTipo(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro no upload do arquivo: {ex.Message}");
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>
        /// Executa uma requisição PUT que retorna dados tipados
        /// </summary>
        /// <typeparam name="TRetorno">Tipo de dados retornados</typeparam>
        /// <param name="dados">Dados para enviar</param>
        /// <param name="url">Endpoint da API</param>
        /// <param name="id">ID do recurso</param>
        /// <returns>Resultado da operação com dados tipados</returns>
        public async Task<RetornoPadrao<TRetorno>> PutAsync<TRetorno>(object dados, string url, string id)
        {
            try
            {
                var json = JsonSerializer.Serialize(dados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{url}/{id}", content);
                return await ProcessarRespostaComTipo<TRetorno>(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<TRetorno>($"Erro na requisição: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição PUT que retorna apenas sucesso/erro
        /// </summary>
        /// <param name="dados">Dados para enviar</param>
        /// <param name="url">Endpoint da API</param>
        /// <param name="id">ID do recurso</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> PutAsync(object dados, string url, string id)
        {
            try
            {
                var json = JsonSerializer.Serialize(dados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{url}/{id}", content);
                return await ProcessarRespostaSemTipo(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro na requisição: {ex.Message}");
            }
        }

        #endregion

        #region GET Methods

        /// <summary>
        /// Executa uma requisição GET que retorna dados tipados
        /// </summary>
        /// <typeparam name="TRetorno">Tipo de dados retornados</typeparam>
        /// <param name="url">Endpoint da API</param>
        /// <returns>Resultado da operação com dados tipados</returns>
        public async Task<RetornoPadrao<TRetorno>> GetAsync<TRetorno>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ProcessarRespostaComTipo<TRetorno>(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<TRetorno>($"Erro na requisição: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição GET que retorna apenas sucesso/erro (para verificações)
        /// </summary>
        /// <param name="url">Endpoint da API</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> GetAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ProcessarRespostaSemTipo(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro na requisição: {ex.Message}");
            }
        }

        #endregion

        #region DELETE Methods

        /// <summary>
        /// Executa uma requisição DELETE que retorna dados tipados
        /// </summary>
        /// <typeparam name="TRetorno">Tipo de dados retornados</typeparam>
        /// <param name="url">Endpoint da API</param>
        /// <param name="id">ID do recurso</param>
        /// <returns>Resultado da operação com dados tipados</returns>
        public async Task<RetornoPadrao<TRetorno>> DeleteAsync<TRetorno>(string url, string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{url}/{id}");
                return await ProcessarRespostaComTipo<TRetorno>(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<TRetorno>($"Erro na requisição: {ex.Message}");
            }
        }

        /// <summary>
        /// Executa uma requisição DELETE que retorna apenas sucesso/erro
        /// </summary>
        /// <param name="url">Endpoint da API</param>
        /// <param name="id">ID do recurso</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> DeleteAsync(string url, string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{url}/{id}");
                return await ProcessarRespostaSemTipo(response);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao($"Erro na requisição: {ex.Message}");
            }
        }

        #endregion

        #region GRAVAR Methods (Inteligente POST/PUT)

        /// <summary>
        /// Grava dados usando POST (criação) ou PUT (atualização) com retorno tipado
        /// </summary>
        /// <typeparam name="TRetorno">Tipo de dados retornados</typeparam>
        /// <param name="dados">Dados para gravar</param>
        /// <param name="url">Endpoint da API</param>
        /// <param name="id">ID para atualização (null = criação)</param>
        /// <returns>Resultado da operação com dados tipados</returns>
        public async Task<RetornoPadrao<TRetorno>> GravarAsync<TRetorno>(object dados, string url, string? id = null)
        {
            return string.IsNullOrEmpty(id) 
                ? await PostAsync<TRetorno>(dados, url)
                : await PutAsync<TRetorno>(dados, url, id);
        }

        /// <summary>
        /// Grava dados usando POST (criação) ou PUT (atualização) sem retorno tipado
        /// </summary>
        /// <param name="dados">Dados para gravar</param>
        /// <param name="url">Endpoint da API</param>
        /// <param name="id">ID para atualização (null = criação)</param>
        /// <returns>Resultado da operação sem dados tipados</returns>
        public async Task<RetornoPadrao> GravarAsync(object dados, string url, string? id = null)
        {
            return string.IsNullOrEmpty(id) 
                ? await PostAsync(dados, url)
                : await PutAsync(dados, url, id);
        }

        #endregion

        #region Métodos Auxiliares Privados

        /// <summary>
        /// Processa a resposta HTTP e converte para RetornoPadrao com tipo
        /// </summary>
        /// <typeparam name="T">Tipo de dados retornados</typeparam>
        /// <param name="response">Resposta HTTP</param>
        /// <returns>Resultado processado com dados tipados</returns>
        private async Task<RetornoPadrao<T>> ProcessarRespostaComTipo<T>(HttpResponseMessage response)
        {
            var resultado = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(resultado))
            {
                return new RetornoPadrao<T>("Resposta vazia do servidor");
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new RetornoPadrao<T>("Token de autenticação inválido ou expirado");
                
                case HttpStatusCode.Forbidden:
                    return new RetornoPadrao<T>("Acesso negado");
                
                case HttpStatusCode.NotFound:
                    return new RetornoPadrao<T>("Recurso não encontrado");
                
                case HttpStatusCode.BadRequest:
                    var retornoBadRequest = JsonSerializer.Deserialize<RetornoPadrao<T>>(resultado, GetJsonOptions());
                    return retornoBadRequest ?? new RetornoPadrao<T>("Dados de entrada inválidos");
            }

            if (!response.IsSuccessStatusCode)
            {
                return new RetornoPadrao<T>($"Erro HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
            }

            try
            {
                var retorno = JsonSerializer.Deserialize<RetornoPadrao<T>>(resultado, GetJsonOptions());
                return retorno ?? new RetornoPadrao<T>("Erro ao processar resposta do servidor");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[DEBUG] Erro de deserialização: {ex.Message}");
                Console.WriteLine($"[DEBUG] JSON recebido: {resultado}");
                return new RetornoPadrao<T>("Formato de resposta inválido do servidor");
            }
        }

        /// <summary>
        /// Processa a resposta HTTP e converte para RetornoPadrao sem tipo
        /// </summary>
        /// <param name="response">Resposta HTTP</param>
        /// <returns>Resultado processado sem dados tipados</returns>
        private async Task<RetornoPadrao> ProcessarRespostaSemTipo(HttpResponseMessage response)
        {
            var resultado = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(resultado))
            {
                return new RetornoPadrao("Resposta vazia do servidor");
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new RetornoPadrao("Token de autenticação inválido ou expirado");
                
                case HttpStatusCode.Forbidden:
                    return new RetornoPadrao("Acesso negado");
                
                case HttpStatusCode.NotFound:
                    return new RetornoPadrao("Recurso não encontrado");
                
                case HttpStatusCode.BadRequest:
                    var retornoBadRequest = JsonSerializer.Deserialize<RetornoPadrao>(resultado, GetJsonOptions());
                    return retornoBadRequest ?? new RetornoPadrao("Dados de entrada inválidos");
            }

            if (!response.IsSuccessStatusCode)
            {
                return new RetornoPadrao($"Erro HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
            }

            try
            {
                var retorno = JsonSerializer.Deserialize<RetornoPadrao>(resultado, GetJsonOptions());
                return retorno ?? new RetornoPadrao("Erro ao processar resposta do servidor");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[DEBUG] Erro de deserialização: {ex.Message}");
                Console.WriteLine($"[DEBUG] JSON recebido: {resultado}");
                return new RetornoPadrao("Formato de resposta inválido do servidor");
            }
        }

        /// <summary>
        /// Executa uma requisição POST para download de arquivo
        /// </summary>
        /// <param name="dados">Dados para enviar</param>
        /// <param name="url">Endpoint da API</param>
        /// <returns>Bytes do arquivo para download</returns>
        public async Task<RetornoPadrao<byte[]>> PostDownloadAsync(object dados, string url)
        {
            try
            {
                var json = JsonSerializer.Serialize(dados);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new RetornoPadrao<byte[]>($"Erro HTTP {(int)response.StatusCode}: {errorContent}");
                }

                // Ler o conteúdo como bytes (para arquivos)
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                return new RetornoPadrao<byte[]>(true, "Download realizado com sucesso", fileBytes);
            }
            catch (Exception ex)
            {
                return new RetornoPadrao<byte[]>($"Erro no download: {ex.Message}");
            }
        }

        /// <summary>
        /// Configurações padrão para serialização JSON
        /// </summary>
        /// <returns>Opções de serialização</returns>
        private static JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null
            };
        }

        #endregion
    }
}