using QMTGroup.DSL.Library.Math;

namespace QMTGroup.DSL.Lua.Test;

public class MathLibExtentionsTest
{
    [Fact]
    public void OnToLua_WhenMathLibrary_ShouldExecuteAllTaggedFeatures()
    {
        // Arrange
        var mathLib = new MathLib();

        // Act
        var mathLuaLib = mathLib.ToLuaLibrary();

        // Assert
        Assert.Equal(33, ((Func<double, double>)mathLuaLib["Abs"]).Invoke(33));
        Assert.Equal(33, ((Func<double, double>)mathLuaLib["Abs"]).Invoke(-33));
    }
}
