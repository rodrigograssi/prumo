using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using PesquisaX.Shared.DTO.Requestes;
using PesquisaX.Shared.DTO.Responses;
using PesquisaX.Shared.Views;

namespace PesquisaX.Web.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AuthService> _logger;
    private UsuarioView? _usuarioAtual;
    private string? _token;

    private const string TOKEN_KEY = "authToken";
    private const string USER_KEY = "currentUser";

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Tentativa de login para: {Email}", request.Email);
            
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            var content = await response.Content.ReadAsStringAsync();
            
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (loginResponse?.Sucesso == true && !string.IsNullOrEmpty(loginResponse.Token))
            {
                // Usar localStorage para manter conectado, sessionStorage para sessão única
                var storage = request.ManterConectado ? "localStorage" : "sessionStorage";
                await SalvarTokenAsync(loginResponse.Token, storage);
                await SalvarUsuarioAsync(loginResponse.Usuario!, storage);
                ConfigurarHttpClientHeaders(loginResponse.Token);
                
                _token = loginResponse.Token;
                _usuarioAtual = loginResponse.Usuario;
                
                _logger.LogInformation("Login realizado com sucesso para: {Email} (Manter conectado: {ManterConectado})", 
                    request.Email, request.ManterConectado);
            }

            return loginResponse ?? new LoginResponse { Sucesso = false, Mensagem = "Erro na comunicação com o servidor" };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede ao fazer login: {Email}", request.Email);
            return new LoginResponse { Sucesso = false, Mensagem = "Erro de conexão com o servidor" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar login: {Email}", request.Email);
            return new LoginResponse { Sucesso = false, Mensagem = "Erro interno. Tente novamente." };
        }
    }

    public async Task<LoginResponse> RegistroAsync(RegistroRequest request)
    {
        try
        {
            _logger.LogInformation("Tentativa de registro para: {Email}", request.Email);
            
            var response = await _httpClient.PostAsJsonAsync("api/auth/registro", request);
            var content = await response.Content.ReadAsStringAsync();
            
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (loginResponse?.Sucesso == true && !string.IsNullOrEmpty(loginResponse.Token))
            {
                await SalvarTokenAsync(loginResponse.Token);
                await SalvarUsuarioAsync(loginResponse.Usuario!);
                ConfigurarHttpClientHeaders(loginResponse.Token);
                
                _token = loginResponse.Token;
                _usuarioAtual = loginResponse.Usuario;
                
                _logger.LogInformation("Registro realizado com sucesso para: {Email}", request.Email);
            }

            return loginResponse ?? new LoginResponse { Sucesso = false, Mensagem = "Erro na comunicação com o servidor" };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede ao fazer registro: {Email}", request.Email);
            return new LoginResponse { Sucesso = false, Mensagem = "Erro de conexão com o servidor" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar registro: {Email}", request.Email);
            return new LoginResponse { Sucesso = false, Mensagem = "Erro interno. Tente novamente." };
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            _logger.LogInformation("Realizando logout");
            
            // Remover de ambos os storages
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", USER_KEY);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", TOKEN_KEY);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", USER_KEY);
            
            _token = null;
            _usuarioAtual = null;
            
            // Remover header de autorização
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar logout");
        }
    }

    public async Task<bool> EstaAutenticadoAsync()
    {
        try
        {
            var token = await ObterTokenDoStorageAsync();
            
            if (string.IsNullOrEmpty(token))
                return false;

            // Verificar se o token não está expirado
            if (TokenEstaExpirado(token))
            {
                await LogoutAsync();
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<UsuarioView?> ObterUsuarioAtualAsync()
    {
        if (_usuarioAtual != null)
            return _usuarioAtual;

        try
        {
            // Tenta localStorage primeiro, depois sessionStorage
            var usuarioJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", USER_KEY);
            if (string.IsNullOrEmpty(usuarioJson))
                usuarioJson = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", USER_KEY);
            
            if (string.IsNullOrEmpty(usuarioJson))
                return null;

            _usuarioAtual = JsonSerializer.Deserialize<UsuarioView>(usuarioJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return _usuarioAtual;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter usuário do storage");
            return null;
        }
    }

    public string? ObterToken()
    {
        return _token;
    }

    private async Task SalvarTokenAsync(string token, string storage = "localStorage")
    {
        await _jsRuntime.InvokeVoidAsync($"{storage}.setItem", TOKEN_KEY, token);
    }

    private async Task SalvarUsuarioAsync(UsuarioView usuario, string storage = "localStorage")
    {
        var usuarioJson = JsonSerializer.Serialize(usuario, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await _jsRuntime.InvokeVoidAsync($"{storage}.setItem", USER_KEY, usuarioJson);
    }

    private async Task<string?> ObterTokenDoStorageAsync()
    {
        try
        {
            // Tenta localStorage primeiro, depois sessionStorage
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);
            if (!string.IsNullOrEmpty(token))
                return token;
                
            return await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", TOKEN_KEY);
        }
        catch
        {
            return null;
        }
    }

    private void ConfigurarHttpClientHeaders(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    private bool TokenEstaExpirado(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            return jwtToken.ValidTo <= DateTime.UtcNow;
        }
        catch
        {
            return true; // Se não conseguir ler o token, considera como expirado
        }
    }

    public async Task<bool> ExistemUsuariosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/auth/has-users");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<bool>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            return true; // Default para não mostrar registro se API falhar
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se existem usuários");
            return true; // Default para não mostrar registro se API falhar
        }
    }

    public async Task InicializarAsync()
    {
        try
        {
            var token = await ObterTokenDoStorageAsync();
            
            if (!string.IsNullOrEmpty(token) && !TokenEstaExpirado(token))
            {
                _token = token;
                ConfigurarHttpClientHeaders(token);
                _usuarioAtual = await ObterUsuarioAtualAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar AuthService");
        }
    }
}