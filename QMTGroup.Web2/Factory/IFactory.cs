namespace QMTGroup.Web.Factory;

public interface IFactory<TInterface> : IEnumerable<KeyValuePair<Guid, TInterface>>
{
    public Guid Create<TInstance>(object? parameters) where TInstance : class, TInterface;

    public TInstance? Get<TInstance>(Guid id) where TInstance : class, TInterface;
}
