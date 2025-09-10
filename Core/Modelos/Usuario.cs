using PesquisaX.Core.ValueObjects;

namespace PesquisaX.Core;

public class Usuario: Entidade
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Senha { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }

    public Sistema? Sistema { get; private set; }
    
    public DateTime DataCriacao { get; private set; } 
    
    protected Usuario() { }
    
    public Usuario(string nome, string email, string senha)
    {
        Nome = nome;
        Email = email;
        Senha = senha;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
    }
    
    public void Alterar(string nome, string email)
    {
        Nome = nome;
        Email = email;
    }
    
    public void AlterarSenha(string senha)
    {
        Senha = senha;
    }
    
    public void DefineAtivo(bool ativo)
    {
        Ativo = ativo;
    }

    public void DefineAcesso(string token)
    {
        Sistema = new Sistema(DateTime.UtcNow, token);   
    }
    
    
    
}