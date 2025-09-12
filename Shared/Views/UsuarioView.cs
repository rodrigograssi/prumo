namespace PesquisaX.Shared.Views;

public class UsuarioView
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = "*MASCARADA*";
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public string Token { get; set; } = string.Empty;
}