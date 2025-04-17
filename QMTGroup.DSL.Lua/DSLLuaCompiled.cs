namespace QMTGroup.DSL.Lua;

public class DSLLuaCompiled : IDisposable
{
    private readonly DSLLuaEngine _engine;

    public required string Name { get; init; }

    public DSLLuaCompiled(DSLLuaEngine engine)
    {
        _engine = engine;
    }

    public void Initialize()
    {
        (_engine.Engine["init"] as NLua.LuaFunction)?.Call();
    }

    public void Execute()
    {
        (_engine.Engine["execute"] as NLua.LuaFunction)?.Call();
    }

    public void Dispose()
    {
        _engine.Engine[Name] = null;
    }
}
