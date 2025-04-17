using Microsoft.Extensions.Logging;

namespace QMTGroup.DSL.Library.Standard;

[DSLNoNamespace]
public class StdLib : IDSLLibrary
{
    private readonly ILogger<StdLib> _logger;

    public StdLib(ILogger<StdLib> logger)
    {
        _logger = logger;
    }

    [DSLFunction]
    public void Print(object value)
    {
        _logger.LogInformation(value.ToString());
    }
}
