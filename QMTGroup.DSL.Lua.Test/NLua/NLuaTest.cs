namespace QMTGroup.DSL.Lua.Test;

public class NLuaTest
{/*
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(1000)]
    public void NluaTest2_HeavyScript2(int loop)
    {
        string scriptStrExec =  "data = {}\r\n" +
                                "function execute()\r\n" +
                                "\tfor i = 1, 5000000 do\r\n" +
                                "\t\tdata[i] = i*7\r\n" +
                                "\tend\r\n" +
                                "end";

        var scr = new DSLLuaScript("nom");
        scr.ExecutionScript = scriptStrExec;

        using (DSLLuaEngine engine = new())
        {
            var compiledScript = engine.Compile(scr);

            for (int i = 0; i < loop; i++)
            {
                compiledScript.Execute();
            }
        }
    }

    [Fact]
    public void NluaTest2_Library()
    {
        string scriptStrExec =  "function execute()\r\n" +
                                "\tMath.Abs(33)\r\n" +
                                "end";

        var scr = new DSLLuaScript("MonScript");
        scr.ExecutionScript = scriptStrExec;

        using (DSLLuaEngine engine = new())
        {
            var compiledScript = engine.Compile(scr);

            compiledScript.Execute();
        }
    }*/
}
