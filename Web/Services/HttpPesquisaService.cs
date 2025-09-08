using System.Net.Http.Json;
using System.Text.Json;
using PesquisaX.Core.Modelos;
using PesquisaX.Shared.DTO;
using PesquisaX.Shared.Views;
using PesquisaX.Web.Servicos;

namespace PesquisaX.Web.Servicos;

public class HttpPesquisaService : IServicoPesquisa
{
    private readonly HttpClient _httpClient;
    private const string API_BASE_URL = "/api/pesquisas";

    public HttpPesquisaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PesquisaListaView>> ObterTodasPesquisasAsync()
    {
        try
        {
            var retorno = await _httpClient.GetFromJsonAsync<List<PesquisaListaView>>(API_BASE_URL);
            return retorno.Data ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<Pesquisa?> ObterPesquisaPorIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Pesquisa>($"{API_BASE_URL}/{id}");
        }
        catch (Exception)
        {
            return null;
        }
    }


    public async Task CriarPesquisaAsync(Pesquisa pesquisa)
    {
        try
        {
            HttpResponseMessage response;
            response = await _httpClient.PostAsJsonAsync(API_BASE_URL, pesquisa);
           
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao salvar pesquisa: {ex.Message}", ex);
        }
    }
    
    public async Task AlterarPesquisaAsync(Pesquisa pesquisa)
    {
        try
        {
            HttpResponseMessage response;
            response = await _httpClient.PutAsJsonAsync($"{API_BASE_URL}/{pesquisa.Id}", pesquisa);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao salvar pesquisa: {ex.Message}", ex);
        }
    }

    public async Task ExcluirPesquisaAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{API_BASE_URL}/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao excluir pesquisa: {ex.Message}", ex);
        }
    }

    public async Task<RespostaPessoa> SalvarResultadoPessoaAsync(Guid pesquisaId, RespostaPessoa resposta)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/{pesquisaId}/resultados", resposta);
            response.EnsureSuccessStatusCode();
            
            var resultadoSalvo = await response.Content.ReadFromJsonAsync<RespostaPessoa>();
            return resultadoSalvo ?? resposta;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao salvar resultado: {ex.Message}", ex);
        }
    }

    public async Task<List<RespostaPessoa>> ObterResultadosPesquisaAsync(Guid pesquisaId)
    {
        try
        {
            var resultados = await _httpClient.GetFromJsonAsync<List<RespostaPessoa>>($"{API_BASE_URL}/{pesquisaId}/resultados");
            return resultados ?? new List<RespostaPessoa>();
        }
        catch (Exception)
        {
            return new List<RespostaPessoa>();
        }
    }

    public async Task<RespostaPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<RespostaPessoa>($"{API_BASE_URL}/{pesquisaId}/resultados/{pessoaId}");
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string> ExportarDadosAsync()
    {
        try
        {
            var pesquisas = await ObterTodasPesquisasAsync();
            return JsonSerializer.Serialize(pesquisas, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao exportar dados: {ex.Message}", ex);
        }
    }

    public async Task ImportarDadosAsync(string dadosJson)
    {
        try
        {
            var pesquisas = JsonSerializer.Deserialize<List<Pesquisa>>(dadosJson);
            if (pesquisas != null)
            {
                foreach (var pesquisa in pesquisas)
                {
                    await CriarPesquisaAsync(pesquisa);
                }
            }
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("Dados inválidos para importação", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao importar dados: {ex.Message}", ex);
        }
    }
}