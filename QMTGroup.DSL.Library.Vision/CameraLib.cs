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
    public LuaTable GetFirst()
    {
        ICamera? cam = _cameraFactory.FirstOrDefault(x => x.Value.Status == CameraStatus.Started).Value ?? _cameraFactory.First().Value;

        var ltable = cam.ToLuaTable();
        ltable = _addGrabFeature(ltable, cam);

        return ltable;
    }

    [DSLConstant]
    public LuaTable Available { get; set; } = new();

    private LuaTable _addGrabFeature(LuaTable lua, ICamera camera)
    {
        camera.OnReciveImage += Camera_OnReciveImage;

        lua["grab"] = () => _grab();
        return lua;
    }
    
    private Image.Matrix? _lastImage = null;

    private string _grab()
    {
        while(_lastImage is null)
        {
            Thread.Sleep(0);
        }

        var result = _lastImage;
        _lastImage = null;

        return result.ToBase64();
    }

    private void Camera_OnReciveImage(object? sender, Image.Matrix e)
    {
        _lastImage = e;
    }
}
