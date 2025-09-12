using System.ComponentModel.DataAnnotations;

namespace PesquisaX.Shared.Comandos;

public class UsuarioCrud
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Senha { get; set; } = string.Empty;
    public string ConfirmarSenha { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}