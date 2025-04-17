using QMTGroup.DSL.Library;

namespace QMTGroup.DSL.Core;

public interface IDSLEngine<TScript> : IDisposable where TScript : class, IDSLScript
{
    public IEnumerable<IDSLLibrary> Libraries { get; }
}
