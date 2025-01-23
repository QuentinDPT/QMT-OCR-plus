using QMTGroup.Image;

namespace QMTGroup.Camera;

public interface ICamera
{
    public event EventHandler<Matrix> OnReciveImage;

    public void StartCapture();

    public void StopCapture();
}
