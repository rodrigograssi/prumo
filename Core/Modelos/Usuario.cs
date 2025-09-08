using System.ComponentModel.DataAnnotations;

namespace PesquisaX.Core;

public class Usuario: Entidade
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}