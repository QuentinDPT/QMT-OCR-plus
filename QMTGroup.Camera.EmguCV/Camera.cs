using Emgu.CV;
using Emgu.CV.CvEnum;
using QMTGroup.Image;

namespace QMTGroup.Camera.EmguCV;

public class Camera : ICamera
{
    private Mat _imageRecived;

    private VideoCapture? _videoCapture = null;

    public event EventHandler<Matrix> OnReciveImage;

    private CameraParameters _cameraParameters;

    public CameraParameters Parameters => _cameraParameters;

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

        _cameraParameters.InternalDefaultParameters.Clear();
        _cameraParameters.InternalDefaultParameters = _extractActualParameters(_videoCapture);

        _videoCapture.FlipVertical = _cameraParameters.FlipVertical;
        _videoCapture.FlipHorizontal = _cameraParameters.FlipHorizontal;

        foreach (var param in _cameraParameters.UserParamters)
        {
            string paramName = param.Key.ToString().Split(":").Last();

            if (!Enum.TryParse(paramName, true, out CapProp paramEnum))
                continue;

            _videoCapture.Set(paramEnum, param.Value);
        }

        _videoCapture.ImageGrabbed += (object sender, EventArgs e) =>
        {
            _videoCapture.Retrieve(_imageRecived);
            OnReciveImage.Invoke(this, _imageRecived.ToMatrix());
        };

        _videoCapture.Start();
    }

    private Dictionary<Urn.Urn, double> _extractActualParameters(VideoCapture videoCapture)
    {
        CapProp[] properties = Enum.GetValues<CapProp>();
        Dictionary<Urn.Urn, double> result = new();

        foreach (CapProp property in properties)
        {
            string propertyName = Enum.GetName(property);
            if (propertyName is null)
                continue;
            Urn.Urn urn = new Urn.Urn($"urn:{propertyName}");
            if (result.ContainsKey(urn))
                continue;
            double value = videoCapture.Get(property);
            result.Add(urn, value);
        }
        return result;
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
