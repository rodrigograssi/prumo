using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.DTO;
using System.Text.Json;

namespace PesquisaX.Web.Services;

public class IAService(ApiController<bool> apiController, ApiController apiControllerBase)
{
    public async Task<bool> EstaDisponivel()
    {
        try
        {
            var response = await apiController.GetAsync("ia/disponivel");
            return response?.Sucesso == true && response.Data;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GerarPerguntas(GerarPerguntasCommand comando)
    {
        try
        {
            var response = await apiControllerBase.PostAsync(comando, "ia/gerar-perguntas");
            return response?.Sucesso == true ? response.Notificacoes?.FirstOrDefault() : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> MelhorarPesquisa(MelhorarPesquisaCommand comando)
    {
        try
        {
            var response = await apiControllerBase.PostAsync(comando, "ia/melhorar-pesquisa");
            return response?.Sucesso == true ? response.Notificacoes?.FirstOrDefault() : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> SugerirPesquisa(SugerirPesquisaCommand comando)
    {
        try
        {
            var response = await apiControllerBase.PostAsync(comando, "ia/sugerir-pesquisa");
            return response?.Sucesso == true ? response.Notificacoes?.FirstOrDefault() : null;
        }
        catch
        {
            return null;
        }
    }
}