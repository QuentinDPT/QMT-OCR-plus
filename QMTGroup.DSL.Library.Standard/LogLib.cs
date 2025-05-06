using Microsoft.Extensions.Logging;
using QMTGroup.Core;

namespace QMTGroup.DSL.Library.Standard;

[DSLNamespace("Logger")]
public class LogLib : IDSLLibrary
{
    private readonly IMemoryLogger _memoryLogger;


    public LogLib(IMemoryLogger memoryLogger)
    {
        _memoryLogger = memoryLogger;
    }

    [DSLFunction]
    public void LogTrace(object value)
    {
        _memoryLogger.LogTrace(value.ToString());
    }

    [DSLFunction]
    public void LogDebug(object value)
    {
        _memoryLogger.LogDebug(value.ToString());
    }

    [DSLFunction]
    public void LogInfo(object value)
    {
        _memoryLogger.LogInformation(value.ToString());
    }

    [DSLFunction]
    public void LogWarning(object value)
    {
        _memoryLogger.LogWarning(value.ToString());
    }

    [DSLFunction]
    public void LogError(object value)
    {
        _memoryLogger.LogError(value.ToString());
    }

    [DSLFunction]
    public void LogCritical(object value)
    {
        _memoryLogger.LogCritical(value.ToString());
    }

    [DSLFunction]
    public void Clear()
    {
        _memoryLogger.Clear();
    }
}
