namespace PesquisaX.Core;

public abstract class Entidade
{
    public Guid Id { get; init; }
    public Entidade()
    {
        Id = Guid.NewGuid();
    }
}