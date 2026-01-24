namespace QMTGroup.DSL.Core;

public interface IDSLScript : IDisposable
{
    public string Name { get; }

    public string ExecutionScript { get; }
}
