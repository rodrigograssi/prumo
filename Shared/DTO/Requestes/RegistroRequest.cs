using System.ComponentModel.DataAnnotations;

namespace Prumo.Shared.DTO.Requestes;

public class RegistroRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Senha { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("Senha", ErrorMessage = "Senhas não conferem")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}