using Microsoft.Extensions.Logging;
using QMTGroup.Core;
using System.Text;

namespace QMTGroup.DSL.Library.Standard;

[DSLNoNamespace]
public class StdLib : IDSLLibrary
{
    private readonly ILogger _logger;

    public StdLib(IWebLogger logger)
    {
        _logger = logger;
    }

    [DSLFunction]
    public void Print(object value)
    {
        _logger.LogInformation(value.ToString());
    }

    [DSLFunction]
    public void Sleep(int time_ms)
    {
        Thread.Sleep(time_ms);
    }

    [DSLFunction]
    public int Rand(int maximum)
    {
        return Random.Shared.Next(maximum);
    }

    [DSLFunction]
    public int Rand(int minimum, int maximum)
    {
        return Random.Shared.Next(minimum, maximum);
    }
}
