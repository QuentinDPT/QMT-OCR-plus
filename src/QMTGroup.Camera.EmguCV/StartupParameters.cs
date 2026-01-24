namespace QMTGroup.Camera.EmguCV;

public record StartupParameters : IStartupParameters
{
    public required int Slot { get; init; }
}
