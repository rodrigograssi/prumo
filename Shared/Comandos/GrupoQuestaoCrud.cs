namespace PesquisaX.Shared.Comandos;

public class GrupoQuestaoCrud
{
    public string Id { get; set; } = string.Empty;
    public string PesquisaId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public string Cor { get; set; } = "#FF6B6B";
    public List<QuestaoCrud> Questoes { get; set; } = [];
}