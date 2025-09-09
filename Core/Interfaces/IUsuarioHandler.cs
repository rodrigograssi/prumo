using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.DTO;
using PesquisaX.Shared.DTO.Responses;

namespace PesquisaX.Core.Interfaces;

public interface IUsuarioHandler
{
    Task<RetornoPadrao<Usuario>> CriarUsuario(UsuarioCrud comando);
    Task<RetornoPadrao<Usuario>> AlterarUsuario(UsuarioCrud comando);
    Task<RetornoPadrao> ExcluirUsuarioAsync(Guid id);
    Task<RetornoPadrao> ResetarSenha(Guid usuarioId, ResetPasswordRequest comando);
}