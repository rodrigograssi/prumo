using PesquisaX.Core;
using PesquisaX.Core.Interfaces;
using PesquisaX.Core.Repositorios;
using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.DTO;
using PesquisaX.Shared.DTO.Responses;

namespace PesquisaX.Infra.Handlers;

public class UsuarioHandler(IRepositorioUsuario repo) : IUsuarioHandler
{
    public async Task<RetornoPadrao<Usuario>> CriarUsuario(UsuarioCrud comando)
    {
        try
        {
            // Verificar se o email já existe
            if (await repo.EmailExisteAsync(comando.Email))
                return new RetornoPadrao<Usuario>("Erro ao criar usuário", "Email já usado");

            // Criar hash da senha
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(comando.Senha);

            // Criar usuário
            var usuario = new Usuario(comando.Nome, comando.Email, senhaHash);

            await repo.GravarAsync(usuario);
            return new RetornoPadrao<Usuario>(true, "Usuário criado com sucesso", usuario);
        }
        catch (Exception ex)
        {
            return new RetornoPadrao<Usuario>("Erro ao cadastra usuario", $"Erro interno do servidor: {ex.Message}");
        }
    }

    public async Task<RetornoPadrao<Usuario>> AlterarUsuario(UsuarioCrud comando)
    {
        if (!Guid.TryParse(comando.Id, out var usuarioId))
            return new RetornoPadrao<Usuario>("Erro ao alterar usuário", "Id no formato errado");
        
        var usuario = await repo.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            return new RetornoPadrao<Usuario>("Erro ao alterar usuário", "Usuário não localizado");
        
        if(usuario.Email != comando.Email && await repo.EmailExisteAsync(comando.Email))
            return new RetornoPadrao<Usuario>("Erro ao alterar usuário", "Email já em uso");
        
        usuario.Alterar(comando.Nome, comando.Email);
        usuario.DefineAtivo(comando.Ativo);

        await repo.GravarAsync(usuario);
        return new RetornoPadrao<Usuario>(true, "Usuário alterado com sucesso", usuario);
    }

    public async Task<RetornoPadrao> ExcluirUsuarioAsync(Guid id)
    {
        var usuario = await repo.ObterPorIdAsync(id);
        if (usuario == null)
            return new RetornoPadrao("Erro ao excluir usuário","Usuário não encontrado");

        // Verificar se não é o último usuário
        var totalUsuarios = await repo.ContarUsuariosAsync();
        if (totalUsuarios <= 1)
            return new RetornoPadrao("Erro ao excluir usuário","Não é possível excluir o último usuário do sistema");

        await repo.ExcluirAsync(id);
        return new RetornoPadrao(true, "Usuário excluído com sucesso");
    }

    public async Task<RetornoPadrao> ResetarSenha(Guid usuarioId, ResetPasswordRequest comando)
    {
        var usuario = await repo.ObterPorIdAsync(usuarioId);
        if (usuario == null)
            return new RetornoPadrao("Erro ao resetar Senha", "Usuário não localizado");

        // Criar hash da nova senha
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(comando.NovaSenha);
        usuario.AlterarSenha(senhaHash); 
        await repo.GravarAsync(usuario);

        return new RetornoPadrao(true, "Senha alterada com sucesso");
    }
}