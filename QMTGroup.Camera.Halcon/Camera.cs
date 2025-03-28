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

    public Camera(ILogger<Camera> logger)
    {
        _logger = logger;
    }

    public void StartCapture()
    {
        _camera = new HFramegrabber();
        try
        {
            _camera.OpenFramegrabber("USB3Vision", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "default", "default", "default", -1, -1);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to open Halcon framegrabber with mode 'USB3Vision'. Switching to 'File' mode as fallback.");
            _camera.OpenFramegrabber("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "default", "default", "default", -1, -1);
        }
        _camera.GrabImageStart(-1);
        _acquisitionTask = _capturePeriodically(_cancellationToken.Token);
    }

    public void StopCapture()
    {
        _cancellationToken.Cancel();
        _acquisitionTask?.Wait();
        _camera?.CloseFramegrabber();
        _cancellationToken = new();
    }

    private async Task _capturePeriodically(CancellationToken tocken)
    {
        try
        {
            while (!tocken.IsCancellationRequested)
            {
                await Task.Run(_captureAndProcessImage);
            }
        }
        catch (TaskCanceledException) { }
    }


    private void _captureAndProcessImage()
    {
        HOperatorSet.GrabImageAsync(out HObject hv_Image, _camera, new HTuple(-1));

        if (hv_Image == null || !hv_Image.IsInitialized())
            return;

        Matrix matrix = _convertHImageToMatrix(new HImage(hv_Image));

        OnReciveImage?.Invoke(null, matrix);
    }

    private static Matrix _convertHImageToMatrix(HImage hv_Image)
    {
        HTuple hv_PtrImage;
        HOperatorSet.GetImagePointer1(hv_Image, out hv_PtrImage, out HTuple type, out HTuple width, out HTuple height);

        byte[] imageData = new byte[width * height];
        Marshal.Copy(hv_PtrImage, imageData, 0, imageData.Length);

        Matrix matrix = new Matrix
        {
            Width = (uint)width.I,
            Height = (uint)height.I,
            Data = imageData,
            ChannelType = typeof(byte)
        };

        return matrix;
    }
}
