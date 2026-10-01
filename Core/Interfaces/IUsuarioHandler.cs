using Prumo.Shared.Comandos;
using Prumo.Shared.DTO;
using Prumo.Shared.DTO.Responses;

namespace Prumo.Core.Interfaces;

public interface IUsuarioHandler
{
    Task<RetornoPadrao<Usuario>> CriarUsuario(UsuarioCrud comando);
    Task<RetornoPadrao<Usuario>> AlterarUsuario(UsuarioCrud comando);
    Task<RetornoPadrao> ExcluirUsuarioAsync(Guid id);
    Task<RetornoPadrao> ResetarSenha(Guid usuarioId, ResetPasswordRequest comando);
}