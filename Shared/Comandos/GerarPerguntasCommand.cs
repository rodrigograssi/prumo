namespace Prumo.Shared.Comandos;

public class GerarPerguntasCommand
{
    public string TemaPesquisa { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int QuantidadePerguntas { get; set; } = 5;
}