namespace PesquisaX.Shared.DTO
{
    public class Paginacao
    {
        public int PaginaAtual { get; set; } = 1;
        public int TotalPaginas => (int)Math.Ceiling(TotalDeRegistros / (double)TamanhoDaPagina);
        public int TamanhoDaPagina { get; set; } = 25;
        public int TotalDeRegistros { get; set; }

        protected Paginacao() { }

        public Paginacao(int totalDeRegistros, int paginaAtual = 1, int tamanhoDaPagina = 25)
        {
            TotalDeRegistros = totalDeRegistros;
            PaginaAtual = paginaAtual;
            TamanhoDaPagina = tamanhoDaPagina;
        }
    }
}
