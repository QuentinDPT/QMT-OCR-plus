namespace LuaOrchestrator.ResourceHub;

public class ComplexObjectResourceHub : IComplexObjectResourceHub
{
    private List<ComplexObjectItem> _items = new List<ComplexObjectItem>();

    public string AddItem<T>(T? obj) where T : notnull
    {
        string key = Guid.NewGuid().ToString();

        _items.Add(new ComplexObjectItem
        {
            Key = key,
            Object = obj,
            Type = typeof(T)
        });

        return key;
    }

    public void UpdateItem<T>(string key, T obj) where T : notnull
    {
        var item = _items.FirstOrDefault(x => x.Key == key);

        if (item == null)
        {
            throw new Exception("Item not found");
        }

        item.Object = obj;
    }

    public T GetItem<T>(string key) where T : notnull
    {
        var item = _items.FirstOrDefault(x => x.Key == key && x.Type == typeof(T));
        if (item == null)
        {
            throw new Exception("Item not found");
        }
        return (T)item.Object;
    }
}
