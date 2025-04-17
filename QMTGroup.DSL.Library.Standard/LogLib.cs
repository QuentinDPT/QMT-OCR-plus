using Microsoft.Extensions.Logging;

namespace QMTGroup.DSL.Library.Standard;

[DSLNamespace("Logger")]
public class LogLib : IDSLLibrary
{
    private readonly ILogger<StdLib> _logger;

    public LogLib(ILogger<StdLib> logger)
    {
        _logger = logger;
    }

    [DSLFunction]
    public void LogTrace(object value)
    {
        _logger.LogTrace(value.ToString());
    }

    [DSLFunction]
    public void LogDebug(object value)
    {
        _logger.LogDebug(value.ToString());
    }

    [DSLFunction]
    public void LogInfo(object value)
    {
        _logger.LogInformation(value.ToString());
    }

    [DSLFunction]
    public void LogWarning(object value)
    {
        _logger.LogWarning(value.ToString());
    }

    [DSLFunction]
    public void LogError(object value)
    {
        _logger.LogError(value.ToString());
    }

    [DSLFunction]
    public void LogCritical(object value)
    {
        _logger.LogCritical(value.ToString());
    }
}
