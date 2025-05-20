using Microsoft.Extensions.Logging;

namespace QMTGroup.DSL.Lua;

public class DSLLuaCompiled : IDisposable
{
    private readonly DSLLuaEngine _engine;
    private readonly ILogger _engineLogger;

    public required string Name { get; init; }

    public DSLLuaCompiled(DSLLuaEngine engine, ILogger engineLogger)
    {
        _engine = engine;
        _engineLogger = engineLogger;
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

    public void Invoke()
    {
        try
        {
            (_engine.Engine["main"] as NLua.LuaFunction)?.Call();
        } catch(Exception ex)
        {
            throw;
        }
    }
}
