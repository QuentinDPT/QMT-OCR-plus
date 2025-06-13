using Emgu.CV;
using Emgu.CV.CvEnum;
using QMTGroup.Image;
using QMTGroup.Urn;

namespace QMTGroup.Camera.EmguCV;

public class Camera : ICamera
{
    private Mat _imageRecived;

    private VideoCapture? _videoCapture = null;

    public event EventHandler<Matrix> OnReciveImage;

    private CameraParameters _cameraParameters;

    public CameraParameters Parameters => _cameraParameters;

    private CameraStatus _cameraStatus = CameraStatus.Stopped;

    public CameraStatus Status => _cameraStatus;

    public Camera(CameraParameters cameraParameters)
    {
        _cameraParameters = cameraParameters;
    }

    public void StartCapture()
    {
        if (_videoCapture is not null)
            return;

        _imageRecived = new Mat();

        Tuple<CapProp, int>[] userProperties = _cameraParameters.UserParamters
            .Select(x => System.Tuple.Create((CapProp)Enum.Parse(typeof(CapProp), x.Key.Last(), true), (int)x.Value))
            .ToArray();

        _videoCapture = new VideoCapture(_cameraParameters.Slot, VideoCapture.API.Any, userProperties);

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
            try
            {
                _videoCapture?.Retrieve(_imageRecived);
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
