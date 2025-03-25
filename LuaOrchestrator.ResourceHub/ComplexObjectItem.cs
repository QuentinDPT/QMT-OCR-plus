namespace LuaOrchestrator.ResourceHub;

internal record ComplexObjectItem
{
    public Type Type { get; set; }

    public object Object { get; set; }

    public string Key { get; set; }
}
