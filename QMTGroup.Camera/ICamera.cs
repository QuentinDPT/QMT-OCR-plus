using QMTGroup.Image;

namespace QMTGroup.Camera;

public interface ICamera
{
    public CameraStatus Status { get; }

    public event EventHandler<Matrix> OnReciveImage;

    public void StartCapture();

    public void StopCapture();
}
