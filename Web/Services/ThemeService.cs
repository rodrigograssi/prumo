using Microsoft.JSInterop;

namespace PesquisaX.Web.Servicos;

public class ServicoTema : IServicoTema
{
    private readonly IJSRuntime _jsRuntime;
    private const string CHAVE_TEMA = "pesquisax_modo_escuro";
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
            var temaArmazenado = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", CHAVE_TEMA);
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
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", CHAVE_TEMA, _eModoEscuro.ToString());
            AoMudarTema?.Invoke();
        }
    }
}