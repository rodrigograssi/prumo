using Prumo.Shared.Views;

namespace Prumo.Shared.DTO.Responses;

public class LoginResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public UsuarioView? Usuario { get; set; }
}