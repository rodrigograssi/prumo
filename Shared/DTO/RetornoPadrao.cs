
namespace PesquisaX.Shared.DTO
{
    public class RetornoPadrao
    {
        public bool Sucesso { get; set; } 
        public string Mensagem { get; set; }  = String.Empty;
        public List<String>? Notificacoes { get;  set; }  = null;

        public RetornoPadrao() { }

        public RetornoPadrao(bool sucesso, string mensagem, List<String>? notificacoes = null)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
            Notificacoes = notificacoes;
        }

        public RetornoPadrao(string erro) : this(false, erro) { }
        public RetornoPadrao(string mensagem, List<String> notificacoes): this(false, mensagem, notificacoes) { }
        // public RetornoPadrao(string mensagem, List<Notification> notificacoes):
        //     this(false, mensagem, notificacoes?.Select(n => n.Message).ToList())
        // {}
        public RetornoPadrao(string mensagem, string erro): this(mensagem, new List<string> { erro }) { }

    }

    public class RetornoPadrao<T>: RetornoPadrao
    {
        public T? Data { get; set; }
        public RetornoPadrao(bool sucesso, string mensagem, T? data, List<String>? notificacoes = null): base(sucesso, mensagem, notificacoes)
        {
            Data = data;
        }

        public RetornoPadrao(): base() {
            Data = default;
        }

        public RetornoPadrao(string erro): base(erro) { }
        public RetornoPadrao(string mensagem, List<String> notificacoes) : base(mensagem, notificacoes) { }
        // public RetornoPadrao(string mensagem, List<Notification> notificacoes) : base(mensagem, notificacoes) { }
        public RetornoPadrao(string mensagem, string erro) : base(mensagem, new List<string> { erro }) { }

    }

    public class RetornoPadraoComPaginacao<T> : RetornoPadrao
    {
        public T? Data { get; set; }
        public Paginacao Paginacao { get; set; } = null!;

        public RetornoPadraoComPaginacao (): base(){}

        public RetornoPadraoComPaginacao (bool sucesso, string msg, T? data, Paginacao paginacao): base(sucesso, msg) {
            Data = data;
            Paginacao = paginacao;
        }

        public RetornoPadraoComPaginacao(string msg): base(msg) { }
    }
}
