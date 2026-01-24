namespace QMTGroup.Camera.File;

public class CameraParameters
{
    /// <summary>
    /// Path to the resouce to display
    /// </summary>
    /// <remarks>Supported formats : <c>png</c>, <c>jpg/jpeg</c></remarks>
    public required string Path { get; set; }

    /// <summary>
    /// Sleep between two images sends in milisecond
    /// </summary>
    public int AcquisitionLoopSleep { get; set; } = 50;
}
