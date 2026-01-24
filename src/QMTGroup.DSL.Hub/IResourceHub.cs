namespace QMTGroup.DSL.Hub;

public interface IResourceHub
{
    public Guid Add<T>(T resx);

    public T? Get<T>(Guid resxId);

    public void Set<T>(Guid resxId, T resx);

    public void Delete(Guid resxId);

    public void Clear();
}
