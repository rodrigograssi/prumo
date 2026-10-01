using Microsoft.JSInterop;
using Prumo.Web.Dto;

namespace Prumo.Web.Servicos;

public class ServicoTema : IServicoTema
{
    private readonly IJSRuntime _jsRuntime;
    
    private bool _eModoEscuro = false;

    public bool EModoEscuro => _eModoEscuro;
    public event Action? AoMudarTema;

    public ServicoTema(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InicializarAsync()
    {
        try
        {
            var temaArmazenado = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", ConfiguracoesWeb.LocalStorageChaveTema);
            _eModoEscuro = !string.IsNullOrEmpty(temaArmazenado) && bool.Parse(temaArmazenado);
        }
        catch
        {
            _eModoEscuro = false; // Padrão modo claro em caso de erro
        }
    }

    public async Task AlternarTemaAsync()
    {
        await DefinirTemaAsync(!_eModoEscuro);
    }

    public async Task DefinirTemaAsync(bool eModoEscuro)
    {
        if (_eModoEscuro != eModoEscuro)
        {
            _eModoEscuro = eModoEscuro;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", ConfiguracoesWeb.LocalStorageChaveTema, _eModoEscuro.ToString());
            AoMudarTema?.Invoke();
        }
    }
}