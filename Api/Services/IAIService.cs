using Prumo.Shared.Comandos;

namespace Prumo.Api.Services;

public interface IAIService
{
    Task<bool> EstaDisponivel();
    Task<string> GerarPerguntas(GerarPerguntasCommand comando);
    Task<string> MelhorarPesquisa(MelhorarPesquisaCommand comando);
    Task<string> SugerirCategoriasEPerguntas(string temaPesquisa, int quantidadeGrupos = 5);
}