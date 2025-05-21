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
        _memoryLogger.LogTrace(value?.ToString() ?? "null");
    }

    [DSLFunction]
    public void LogDebug(object value)
    {
        _memoryLogger.LogDebug(value?.ToString() ?? "null");
    }

    [DSLFunction]
    public void LogInfo(object value)
    {
        _memoryLogger.LogInformation(value?.ToString() ?? "null");
    }

    [DSLFunction]
    public void LogWarning(object value)
    {
        _memoryLogger.LogWarning(value?.ToString() ?? "null");
    }

    [DSLFunction]
    public void LogError(object value)
    {
        _memoryLogger.LogError(value?.ToString() ?? "null");
    }

    [DSLFunction]
    public void LogCritical(object value)
    {
        _memoryLogger.LogCritical(value?.ToString() ?? "null");
    }

    [DSLFunction]
    public void Clear()
    {
        //_memoryLogger.Clear();
    }
}
