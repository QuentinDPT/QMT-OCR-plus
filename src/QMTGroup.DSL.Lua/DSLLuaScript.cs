using QMTGroup.DSL.Core;

namespace QMTGroup.DSL.Lua;

public class DSLLuaScript : IDSLScript
{
    public string ExecutionScript { get; set; } = string.Empty;

    public string Name { get; private set; }

    public DSLLuaScript(string name)
    {
        Name = name;
    }

    public void Dispose()
    { }
}
