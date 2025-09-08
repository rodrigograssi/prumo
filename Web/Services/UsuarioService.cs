using PesquisaX.Core;
using System.Net.Http.Json;
using System.Text.Json;
using PesquisaX.Shared.DTO.Requestes;
using PesquisaX.Shared.DTO.Responses;

namespace PesquisaX.Web.Services;

public class UsuarioService : IUsuarioService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(HttpClient httpClient, ILogger<UsuarioService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Usuario>> ObterUsuariosAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/usuarios?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var usuarios = JsonSerializer.Deserialize<List<Usuario>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return usuarios ?? new List<Usuario>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter usuários");
            throw;
        }
    }

    public async Task<Usuario?> ObterUsuarioAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/usuarios/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Usuario>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter usuário {Id}", id);
            throw;
        }
    }

    public async Task<Usuario> CriarUsuarioAsync(RegistroRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/usuarios", request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var usuario = JsonSerializer.Deserialize<Usuario>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return usuario ?? throw new InvalidOperationException("Usuário criado é nulo");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário");
            throw;
        }
    }

    public async Task<Usuario> AtualizarUsuarioAsync(int id, Usuario usuario)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/usuarios/{id}", usuario);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var usuarioAtualizado = JsonSerializer.Deserialize<Usuario>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return usuarioAtualizado ?? throw new InvalidOperationException("Usuário atualizado é nulo");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário {Id}", id);
            throw;
        }
    }

    public async Task ExcluirUsuarioAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/usuarios/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir usuário {Id}", id);
            throw;
        }
    }

    public async Task ResetarSenhaAsync(int id, ResetPasswordRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"api/usuarios/{id}/reset-password", request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao resetar senha do usuário {Id}", id);
            throw;
        }
    }

    public async Task<int> ContarUsuariosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/usuarios/count");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<int>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao contar usuários");
            return 0;
        }
    }
}