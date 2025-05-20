using QMTGroup.DSL.Library.Standard.Model;

namespace QMTGroup.DSL.Library.Standard;

[DSLNamespace("Time")]
public class TimeLib : IDSLLibrary
{
    [DSLFunction]
    public LuaTime From(int year, int month, int day, int hour = 0, int minute = 0, int seconds = 0)
    {
        return new LuaTime(year, month, day, hour, minute, seconds);
    }

    [DSLFunction]
    public LuaTime Now()
    {
        var now = DateTime.Now;
        return From(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    }
}
