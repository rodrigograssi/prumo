namespace PesquisaX.Web.Servicos;

public interface IServicoTema
{
    bool EModoEscuro { get; }
    event Action? AoMudarTema;
    Task InicializarAsync();
    Task AlternarTemaAsync();
    Task DefinirTemaAsync(bool eModoEscuro);
}