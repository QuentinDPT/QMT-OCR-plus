using QMTGroup.DSL.Library;

namespace QMTGroup.DSL.Lua.Test;

public class IDSLLibraryExtentionsTest
{
    [Fact]
    public void OnToLua_WhenLibraryValid_ShouldExecuteAllTaggedFeatures()
    {
        // Arrange
        IDSLLibrary lib = new LibImplementation();

        // Act
        var luaLib = lib.ToLuaLibrary();

        // Assert
        ((Action)luaLib["TEST"]).Invoke();
        ((Action<int>)luaLib["TEST2"]).Invoke(33);
        Assert.Equal(15, ((Func<int>)luaLib["TEST3"]).Invoke());
        Assert.Equal(4, ((Func<string, int>)luaLib["TEST4"]).Invoke("1234"));
        Assert.Null(luaLib["NON_TAGGED"]);
    }
}

[DSLNamespace("Test")]
public class LibImplementation : IDSLLibrary
{
    [DSLFunction]
    public void TEST() { }

    [DSLFunction]
    public void TEST2(int i) { }

    [DSLFunction]
    public int TEST3() => 15;

    [DSLFunction]
    public int TEST4(string str) => str.Length;

    public int NON_TAGGED(string str) => str.Length;
}