using Prumo.Shared.Comandos;

namespace Prumo.Web.Services;

public class IAService(ApiControllerGenerico apiController)
{
    public async Task<bool> EstaDisponivel()
    {
        try
        {
            var response = await apiController.GetAsync("api/IA/disponivel");
            return response?.Sucesso ?? false;
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
            var response = await apiController.PostAsync(comando, "api/IA/gerar-perguntas");
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
            var response = await apiController.PostAsync(comando, "api/IA/melhorar-pesquisa");
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
            var response = await apiController.PostAsync(comando, "api/IA/sugerir-pesquisa");
            return response?.Sucesso == true ? response.Notificacoes?.FirstOrDefault() : null;
        }
        catch
        {
            return null;
        }
    }
}