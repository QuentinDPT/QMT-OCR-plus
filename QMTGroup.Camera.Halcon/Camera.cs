using HalconDotNet;
using Microsoft.Extensions.Logging;
using QMTGroup.Image;
using System.Runtime.InteropServices;

namespace QMTGroup.Camera.Halcon;

public class Camera : ICamera
{
    public event EventHandler<Matrix> OnReciveImage;

    private HFramegrabber? _camera = null;

    private CancellationTokenSource _cancellationToken = new();

    private Task? _acquisitionTask;

    private readonly ILogger _logger;

    public IEnumerable<string> Parameters => _parameters;
    private List<string> _parameters = new();


    public Camera(ILogger<Camera> logger)
    {
        _logger = logger;
    }


    public void StartCapture()
    {
        if (_camera is not null)
            return;

        _camera = new HFramegrabber();
        try
        {
            _camera.OpenFramegrabber("USB3Vision", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "default", "default", "default", -1, -1);

            // extract camera parameter
            HOperatorSet.InfoFramegrabber(new HTuple("USB3Vision"), new HTuple("info_boards"), out HTuple infosKeys, out HTuple infoValues);
            // > paramètres généraux, quelles sont les camera par exemple

            HOperatorSet.InfoFramegrabber(new HTuple("USB3Vision"), new HTuple("parameters"), out HTuple infosKeys2, out HTuple infoValues2);
            // > parametres intérresants, mais pas la totalité

            HOperatorSet.GetFramegrabberParam(_camera, new HTuple("available_param_names"), out HTuple infoValues3);
            // > paramètres de la camera. le plus intérressant pour nous.

            _parameters = infoValues3.SArr.Distinct().OrderBy(x => x).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to open Halcon framegrabber with mode 'USB3Vision'. Switching to 'File' mode as fallback.");
            _camera.OpenFramegrabber("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "default", "default", "default", -1, -1);
        }

        _camera.GrabImageStart(-1);
        _acquisitionTask = Task.Run(() => _capturePeriodically(_cancellationToken.Token));
    }

    public void StopCapture()
    {
        if (_camera is null)
            return;

        _cancellationToken.Cancel();
        _acquisitionTask?.Wait();
        DisposeV2();
        _camera.CloseFramegrabber();
        _camera.Dispose();
        _camera = null;
        _cancellationToken.Dispose();
        _cancellationToken = new();
    }

    private void _capturePeriodically(CancellationToken tocken)
    {
        try
        {
            while (!tocken.IsCancellationRequested)
            {
                _captureV2();
            }
        }
        catch (TaskCanceledException) { }
        catch (Exception ex) {
            _logger.LogError(ex, "Error when executing the camera aquisition thread.");
            if (_camera is not null)
                _camera.CloseFramegrabber();
            _cancellationToken = new();
            _camera = null;
        }
    }

    private HObject _obj = new HObject();
    private HImage _img = new HImage();
    private HTuple tupleNeg = new HTuple(-1);
    private Matrix? matt;

    public void _captureV2()
    {
        _obj?.Dispose();
        HOperatorSet.GrabImageAsync(out _obj, _camera, tupleNeg);

        if (_obj == null || !_obj.IsInitialized())
            return;

        _img?.Dispose();
        _img = new HImage(_obj);

        matt ??= new Matrix();

        _convertHImageToMatrix(ref matt, _img);

        OnReciveImage?.Invoke(null, matt);
    }

    public void DisposeV2()
    {
        _obj?.Dispose();
        _obj = null;
        _img?.Dispose();
        _img = null;
    }

    private void _captureAndProcessImage()
    {
        HOperatorSet.GrabImageAsync(out _obj, _camera, new HTuple(-1));

        if (_obj == null || !_obj.IsInitialized())
            return;

        _img?.Dispose();
        _img = new HImage(_obj);

        matt = new Matrix();
        _convertHImageToMatrix(ref matt, _img);

        OnReciveImage?.Invoke(null, matt);
    }

    private HTuple hv_ptrImage;
    private HTuple ht_width;
    private HTuple ht_height;
    private int requiredImageSize;

    private CameraStatus _status = CameraStatus.Stopped;

    public CameraStatus Status => _status;

    private void _convertHImageToMatrix(ref Matrix matrix, HImage hv_Image)
    {
        hv_ptrImage?.Dispose();
        hv_ptrImage = new HTuple();
        ht_width?.Dispose();
        ht_width = new HTuple();
        ht_height?.Dispose();
        ht_height = new HTuple();

        HOperatorSet.GetImagePointer1(hv_Image, out hv_ptrImage, out HTuple _, out ht_width, out ht_height);

        requiredImageSize = ht_width.I * ht_height.I;

        if(requiredImageSize != matrix.Data.Length)
        {
            matrix.SetDataSize(requiredImageSize, ht_width.I, ht_height.I, DataType.Y_8);
            matrix.SetData(matrix.Data);
        }

        matrix.SetData(hv_ptrImage);
    }
}
