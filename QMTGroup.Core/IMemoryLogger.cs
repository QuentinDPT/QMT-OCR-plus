using Microsoft.Extensions.Logging;

namespace QMTGroup.Core;

public interface IMemoryLogger : ILogger
{
    public void Clear();
}
