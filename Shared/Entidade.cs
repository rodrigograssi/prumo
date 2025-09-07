namespace PesquisaX.Shared;

public abstract class Entidade
{
    public Guid Id { get; set; } =  Guid.NewGuid();
}