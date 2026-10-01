// using System.Net.Http.Json;
// using System.Text.Json;
// using Prumo.Shared.Comandos;
// using Prumo.Shared.DTO.Requestes;
// using Prumo.Shared.DTO.Responses;
// using Prumo.Shared.Views;
//
// namespace Prumo.Web.Services;
//
// public class UsuarioServico : IUsuarioServico
// {
//     private readonly HttpClient _httpClient;
//     private readonly ILogger<UsuarioServico> _logger;
//
//     public UsuarioServico(HttpClient httpClient, ILogger<UsuarioServico> logger)
//     {
//         _httpClient = httpClient;
//         _logger = logger;
//     }
//
//     public async Task<List<UsuarioView>> ObterUsuariosAsync(int page = 1, int pageSize = 10)
//     {
//         try
//         {
//             var response = await _httpClient.GetAsync($"api/usuarios?page={page}&pageSize={pageSize}");
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//             var usuarios = JsonSerializer.Deserialize<List<UsuarioView>>(content, new JsonSerializerOptions
//             {
//                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//             });
//
//             return usuarios ?? new List<UsuarioView>();
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Erro ao obter usuários");
//             throw;
//         }
//     }
//
//     public async Task<UsuarioView?> ObterUsuarioAsync(int id)
//     {
//         try
//         {
//             var response = await _httpClient.GetAsync($"api/usuarios/{id}");
//             if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
//             {
//                 return null;
//             }
//
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//             return JsonSerializer.Deserialize<UsuarioView>(content, new JsonSerializerOptions
//             {
//                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Erro ao obter usuário {Id}", id);
//             throw;
//         }
//     }
//
//     public async Task CriarUsuarioAsync(UsuarioCrud request)
//     {
//         try
//         {
//             var response = await _httpClient.PostAsJsonAsync("api/usuarios", request);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//             var usuario = JsonSerializer.Deserialize<UsuarioView>(content, new JsonSerializerOptions
//             {
//                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Erro ao criar usuário");
//             throw;
//         }
//     }
//
//     public async Task AtualizarUsuarioAsync(int id, UsuarioCrud usuario)
//     {
//         try
//         {
//             var response = await _httpClient.PutAsJsonAsync($"api/usuarios/{id}", usuario);
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//             var usuarioAtualizado = JsonSerializer.Deserialize<UsuarioView>(content, new JsonSerializerOptions
//             {
//                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//             });
//
//             return usuarioAtualizado ?? throw new InvalidOperationException("Usuário atualizado é nulo");
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Erro ao atualizar usuário {Id}", id);
//             throw;
//         }
//     }
//
//     public async Task ExcluirUsuarioAsync(int id)
//     {
//         try
//         {
//             var response = await _httpClient.DeleteAsync($"api/usuarios/{id}");
//             response.EnsureSuccessStatusCode();
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Erro ao excluir usuário {Id}", id);
//             throw;
//         }
//     }
//
//     public async Task ResetarSenhaAsync(int id, ResetPasswordRequest request)
//     {
//         try
//         {
//             var response = await _httpClient.PostAsJsonAsync($"api/usuarios/{id}/reset-password", request);
//             response.EnsureSuccessStatusCode();
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Erro ao resetar senha do usuário {Id}", id);
//             throw;
//         }
//     }
//
//     public async Task<int> ContarUsuariosAsync()
//     {
//         try
//         {
//             var response = await _httpClient.GetAsync("api/usuarios/count");
//             response.EnsureSuccessStatusCode();
//
//             var content = await response.Content.ReadAsStringAsync();
//             return JsonSerializer.Deserialize<int>(content, new JsonSerializerOptions
//             {
//                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Erro ao contar usuários");
//             return 0;
//         }
//     }
// }