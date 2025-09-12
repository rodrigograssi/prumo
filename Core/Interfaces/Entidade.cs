namespace PesquisaX.Core;

public abstract class Entidade
{
    public int Id { get; protected set; }
    public Guid PublicId { get; init; }
    protected Entidade(int id)
    {
        Id = id;
        PublicId = Guid.NewGuid();
    }
    protected Entidade()
    {
        PublicId = Guid.NewGuid();
    }
}