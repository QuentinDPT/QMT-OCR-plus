using Microsoft.Extensions.Logging;
using QMTGroup.Core;

namespace QMTGroup.DSL.Library.Standard;

[DSLNamespace("Logger")]
public class LogLib : IDSLLibrary
{
    private readonly IWebLogger _memoryLogger;

    public LogLib(IWebLogger memoryLogger)
    {
        _memoryLogger = memoryLogger;
    }

    [DSLFunction]
    public void LogTrace(object value)
    {
        _memoryLogger.LogTrace(value?.ToHTML());
    }

    [DSLFunction]
    public void LogDebug(object value)
    {
        _memoryLogger.LogDebug(value?.ToHTML());
    }

    [DSLFunction]
    public void LogInfo(object value)
    {
        _memoryLogger.LogInformation(value?.ToHTML());
    }

    [DSLFunction]
    public void LogWarning(object value)
    {
        _memoryLogger.LogWarning(value?.ToHTML());
    }

    [DSLFunction]
    public void LogError(object value)
    {
        _memoryLogger.LogError(value?.ToHTML());
    }

    [DSLFunction]
    public void LogCritical(object value)
    {
        _memoryLogger.LogCritical(value?.ToHTML());
    }

    [DSLFunction]
    public void Clear()
    {
        //_memoryLogger.Clear();
    }
}
