namespace PesquisaX.Shared.Views;

public class PesquisaResultadoView
{
    public string Nome { get; set; }
    public string NomeDoResultadoDesejado { get; set; }
    public List<CategoriaView> Categorias { get; set; } = [];
}

public class CategoriaView
{
    public string Nome { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public double Total { get; set; }
}