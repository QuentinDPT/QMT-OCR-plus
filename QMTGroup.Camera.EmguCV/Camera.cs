using Emgu.CV;
using QMTGroup.Image;

namespace QMTGroup.Camera.EmguCV;

public class Camera : ICamera
{
    private Mat _imageRecived;

    private VideoCapture? _videoCapture = null;

    public event EventHandler<Matrix> OnReciveImage;

    private CameraParameters _cameraParameters;

    public Camera(CameraParameters cameraParameters)
    {
        _cameraParameters = cameraParameters;
    }

    public void StartCapture()
    {
        if (_videoCapture is not null)
            return;
        
        _imageRecived = new Mat();
        _videoCapture = new VideoCapture(_cameraParameters.Slot);

        _videoCapture.ImageGrabbed += (object sender, EventArgs e) =>
        {
            _videoCapture.Retrieve(_imageRecived);
            OnReciveImage.Invoke(this, _imageRecived.ToMatrix());
        };

        _videoCapture.Start();
    }

    public void StopCapture()
    {
        if (_videoCapture is null)
            return;

        _videoCapture.Stop();
        try
        {
            _videoCapture.Dispose();
        }
        catch (Exception)
        {
            _videoCapture.Dispose();
        }
        _videoCapture = null;
    }
}
