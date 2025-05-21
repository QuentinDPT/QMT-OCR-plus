using Neo.IronLua;
using QMTGroup.Camera;
using QMTGroup.Core;
using QMTGroup.DSL.Lua;

namespace QMTGroup.DSL.Library.Vision;

[DSLNamespace("Camera")]
public class CameraLib : IDSLLibrary
{
    private readonly IWebLogger _webLogger;
    private readonly ICameraFactory _cameraFactory;

    public CameraLib(IWebLogger webLogger, ICameraFactory cameraFactory)
    {
        _webLogger = webLogger;
        _cameraFactory = cameraFactory;
    }

    [DSLFunction]
    public LuaTable Get(object value)
    {
        if (value is not string str)
            return new LuaTable();

        return _cameraFactory.Get<ICamera>(Guid.Parse(str))?.ToLuaTable() ?? new LuaTable();
    }

    [DSLFunction]
    public LuaTable GetFirst() => _cameraFactory.First().Value.ToLuaTable();

    [DSLConstant]
    public LuaTable Available { get; set; } = new();
}
