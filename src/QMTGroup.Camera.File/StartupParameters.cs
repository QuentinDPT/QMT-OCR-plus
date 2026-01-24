namespace QMTGroup.Camera.File;

public class StartupParameters : IStartupParameters
{
    public required string FileLocation { get; set; }

    public int AcquisitionLoopSleep { get; set; } = 50;
}
