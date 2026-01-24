namespace LuaOrchestrator.ResourceHub;

public interface IComplexObjectResourceHub
{
    string AddItem<T>(T? obj) where T : notnull;

    void UpdateItem<T>(string key, T obj) where T : notnull;

    T GetItem<T>(string key) where T : notnull;
}
