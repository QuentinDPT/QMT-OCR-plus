using QMTGroup.Image;

namespace QMTGroup.Camera;

public interface ICamera
{
    public PostAcquisitionParameters PostAcquisitionParameters { get; }

    public IStartupParameters StartupParameters { get; }

    public CameraStatus Status { get; }

    public event EventHandler<Matrix> OnReciveImage;

    public void StartCapture();

    public void StopCapture();
}
