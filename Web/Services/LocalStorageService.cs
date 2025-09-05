using Microsoft.JSInterop;
using System.Text.Json;
using PesquisaX.Shared.Modelos;

namespace PesquisaX.Web.Servicos;

public class LocalStorageService : IServicoPesquisa
{
    private readonly IJSRuntime _jsRuntime;
    private const string CHAVE_PESQUISAS = "pesquisax_pesquisas";
    
    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task<List<Pesquisa>> ObterTodasPesquisasAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", CHAVE_PESQUISAS);
            if (string.IsNullOrEmpty(json))
                return new List<Pesquisa>();
                
            return JsonSerializer.Deserialize<List<Pesquisa>>(json) ?? new List<Pesquisa>();
        }
        catch
        {
            return new List<Pesquisa>();
        }
    }
    
    public async Task<Pesquisa?> ObterPesquisaPorIdAsync(Guid id)
    {
        var pesquisas = await ObterTodasPesquisasAsync();
        return pesquisas.FirstOrDefault(s => s.Id == id);
    }
    
    public async Task SalvarPesquisaAsync(Pesquisa pesquisa)
    {
        var pesquisas = await ObterTodasPesquisasAsync();
        var existingIndex = pesquisas.FindIndex(s => s.Id == pesquisa.Id);
        
        if (existingIndex >= 0)
            pesquisas[existingIndex] = pesquisa;
        else
            pesquisas.Add(pesquisa);
            
        await SalvarPesquisasAsync(pesquisas);
    }
    
    public async Task ExcluirPesquisaAsync(Guid id)
    {
        var pesquisas = await ObterTodasPesquisasAsync();
        pesquisas.RemoveAll(s => s.Id == id);
        await SalvarPesquisasAsync(pesquisas);
    }
    
    public async Task<ResultadoPessoa> SalvarResultadoPessoaAsync(Guid pesquisaId, ResultadoPessoa resultado)
    {
        var pesquisa = await ObterPesquisaPorIdAsync(pesquisaId);
        if (pesquisa == null)
            throw new ArgumentException("Pesquisa não encontrada");
            
        var existingIndex = pesquisa.Resultados.FindIndex(r => r.Id == resultado.Id);
        if (existingIndex >= 0)
            pesquisa.Resultados[existingIndex] = resultado;
        else
            pesquisa.Resultados.Add(resultado);
            
        await SalvarPesquisaAsync(pesquisa);
        return resultado;
    }
    
    public async Task<List<ResultadoPessoa>> ObterResultadosPesquisaAsync(Guid pesquisaId)
    {
        var pesquisa = await ObterPesquisaPorIdAsync(pesquisaId);
        return pesquisa?.Resultados ?? new List<ResultadoPessoa>();
    }
    
    public async Task<ResultadoPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId)
    {
        var pesquisa = await ObterPesquisaPorIdAsync(pesquisaId);
        return pesquisa?.Resultados.FirstOrDefault(r => r.Id == pessoaId);
    }
    
    public async Task<string> ExportarDadosAsync()
    {
        var pesquisas = await ObterTodasPesquisasAsync();
        return JsonSerializer.Serialize(pesquisas, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }
    
    public async Task ImportarDadosAsync(string dadosJson)
    {
        try
        {
            var pesquisas = JsonSerializer.Deserialize<List<Pesquisa>>(dadosJson);
            if (pesquisas != null)
                await SalvarPesquisasAsync(pesquisas);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("Dados inválidos para importação", ex);
        }
    }
    
    private async Task SalvarPesquisasAsync(List<Pesquisa> pesquisas)
    {
        var json = JsonSerializer.Serialize(pesquisas);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", CHAVE_PESQUISAS, json);
    }
}