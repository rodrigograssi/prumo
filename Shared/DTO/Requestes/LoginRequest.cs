using System.ComponentModel.DataAnnotations;

namespace Prumo.Shared.DTO.Requestes;

public class LoginRequest
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    public string Senha { get; set; } = string.Empty;
    
    public bool ManterConectado { get; set; } = false;
}