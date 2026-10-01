using System.Net.Http.Headers;
using Prumo.Web.Dto;

namespace Prumo.Web.Services;

public class AuthenticationHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Debug: Verificar estado do token
        var token = ConfiguracoesWeb.Token;
        Console.WriteLine($"[DEBUG AuthHandler] Token disponível: {!string.IsNullOrEmpty(token)}");
        Console.WriteLine($"[DEBUG AuthHandler] URL: {request.RequestUri}");
        
        // Adicionar token automaticamente se estiver disponível
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"[DEBUG AuthHandler] Authorization header adicionado: Bearer {token[..Math.Min(token.Length, 20)]}...");
        }
        else
        {
            Console.WriteLine("[DEBUG AuthHandler] Nenhum token disponível - requisição sem autorização");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}