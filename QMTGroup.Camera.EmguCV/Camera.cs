using Emgu.CV;
using QMTGroup.Image;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace QMTGroup.Camera.EmguCV;

public class Camera : ICamera
{
    private Mat _imageRecived;

    private VideoCapture _videoCapture;

    public event EventHandler<Matrix> OnReciveImage;

    private CameraParameters _cameraParameters;

    public Camera(CameraParameters cameraParameters)
    {
        _cameraParameters = cameraParameters;
    }

    public void StartCapture()
    {
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
        _videoCapture.Stop();
    }
}
