
namespace QMTGroup.DSL.Hub;

public class ResourceHub : IResourceHub
{
    private Dictionary<Guid, object> _keyValuePairs = new();

    public Guid Add<T>(T resx)
    {
        ArgumentNullException.ThrowIfNull(resx);

        var uuid = Guid.NewGuid();

        _keyValuePairs.Add(uuid, resx);

        return uuid;
    }

    public void Clear()
    {
        _keyValuePairs.Clear();
    }

    public void Delete(Guid resxId)
    {
        _keyValuePairs.Remove(resxId);
    }

    public T? Get<T>(Guid resxId)
    {
        if (!_keyValuePairs.TryGetValue(resxId, out object? value) || value is null)
            return default;

        if (value is T obj)
            return obj;

        return default;
    }

    public void Set<T>(Guid resxId, T resx)
    {
        ArgumentNullException.ThrowIfNull(resx);

        if (_keyValuePairs.ContainsKey(resxId))
            _keyValuePairs[resxId] = resx;
        else
            _keyValuePairs.Add(resxId, resx);
    }
}
