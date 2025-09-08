namespace PesquisaX.Shared.DTO;

public class PaginacaoRequest
{
    public int PaginaAtual { get; set; } = 1;
    public int TamanhoDaPagina { get; set; } = 25;
}