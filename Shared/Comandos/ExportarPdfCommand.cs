using PesquisaX.Shared.Views;

namespace PesquisaX.Shared.Comandos;

public class ExportarPdfCommand
{
    public string ChartImageBase64 { get; set; } = string.Empty;
    public string NomePessoa { get; set; } = string.Empty;
    public string NomePesquisa { get; set; } = string.Empty;
    public DateTime DataAvaliacao { get; set; }
    public List<MediaDeResultadosDosGruposView> DetalhesGrupos { get; set; } = new();
}