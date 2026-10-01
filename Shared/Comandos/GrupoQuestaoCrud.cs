namespace Prumo.Shared.Comandos;

public class GrupoQuestaoCrud
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public int PesquisaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public string Cor { get; set; } = "#FF6B6B";
    public List<QuestaoCrud> Questoes { get; set; } = [];
}