using System.ComponentModel.DataAnnotations;

namespace PesquisaX.Shared.DTO.Responses;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Nova senha deve ter no mínimo 6 caracteres")]
    public string NovaSenha { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("NovaSenha", ErrorMessage = "Senhas não conferem")]
    public string ConfirmarNovaSenha { get; set; } = string.Empty;
}