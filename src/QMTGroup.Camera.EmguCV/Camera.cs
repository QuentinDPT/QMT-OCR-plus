using Emgu.CV;
using Emgu.CV.CvEnum;
using Microsoft.Extensions.Logging;
using QMTGroup.Image;
using QMTGroup.Urn;

namespace QMTGroup.Camera.EmguCV;

public class Camera : ICamera
{
    private Mat _imageRecived;

    private VideoCapture? _videoCapture = null;

    public event EventHandler<Matrix> OnReciveImage;

    private CameraStatus _cameraStatus = CameraStatus.Stopped;

    public CameraStatus Status => _cameraStatus;

    public PostAcquisitionParameters PostAcquisitionParameters => _postAcquisitionParameters;

    public IStartupParameters StartupParameters  => _startupParameters;
    private StartupParameters _startupParameters;

    public PostAcquisitionParameters _postAcquisitionParameters = new();

    private ILogger<ICamera> _cameraLogger;

    public Camera(PostAcquisitionParameters postAcquisitionParameters, StartupParameters startupParameters, ILogger<Camera> logger)
    {
        _cameraLogger = logger;
        _postAcquisitionParameters = _postAcquisitionParameters;
        _startupParameters = startupParameters;
    }

    public void StartCapture()
    {
        if (_videoCapture is not null)
            return;

        _imageRecived = new Mat();

        // todo : userparameters
        Tuple<CapProp, int>[] userProperties = [];

        // todo : settableparameters extration

        _videoCapture = new VideoCapture(_startupParameters.Slot, VideoCapture.API.Any, userProperties);

        bool vFlip = _postAcquisitionParameters.VerticalFlip;
        bool hFlip = _postAcquisitionParameters.HorizontalFlip;

        if (PostAcquisitionParameters.Rotation == QuarterRotation.Deg180)
        {
            hFlip = !hFlip;
            vFlip = !vFlip;
        }

        _videoCapture.FlipVertical = vFlip;
        _videoCapture.FlipHorizontal = hFlip;

        _videoCapture.ImageGrabbed += (object sender, EventArgs e) =>
        {
            try
            {
                _videoCapture?.Retrieve(_imageRecived);

                if (PostAcquisitionParameters.ForceGrayScale)
                {
                    CvInvoke.CvtColor(_imageRecived, _imageRecived, ColorConversion.Bgr2Gray);
                }

                switch (PostAcquisitionParameters.Rotation)
                {
                    case QuarterRotation.Deg90:
                        CvInvoke.Rotate(_imageRecived, _imageRecived, RotateFlags.Rotate90Clockwise);
                        break;
                    case QuarterRotation.Deg270:
                        CvInvoke.Rotate(_imageRecived, _imageRecived, RotateFlags.Rotate90CounterClockwise);
                        break;
                }

                OnReciveImage?.Invoke(this, _imageRecived.ToMatrix());
            }
            catch (Exception)
            {
                _internalDispose();
            }
        };

        _videoCapture.Start();

        _cameraStatus = CameraStatus.Started;
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

        _videoCapture.Release();

        try
        {
            _videoCapture.Dispose();
        }
        catch (Exception)
        {
            _videoCapture.Dispose();
        }

        _videoCapture = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        _cameraStatus = CameraStatus.Stopped;
    }

    private void _internalDispose()
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
