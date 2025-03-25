using HalconDotNet;
using QMTGroup.Image;
using System.Runtime.InteropServices;

namespace QMTGroup.Camera.Halcon;

public class Camera : ICamera
{
    public event EventHandler<Matrix> OnReciveImage;

    private HFramegrabber? _camera = null;

    private CancellationTokenSource _cancellationToken = new();

    private Task? _acquisitionTask;

    public void StartCapture()
    {
        _camera = new HFramegrabber();
        _camera.OpenFramegrabber("USB3", 1,1,0,0,0,0,"default",-1,"default",-1,"default","default","default",-1,-1);
        _camera.GrabImageStart(-1);
        _acquisitionTask = _capturePeriodically(_cancellationToken.Token);
    }

    public void StopCapture()
    {
        _cancellationToken.Cancel();
        _acquisitionTask?.Wait();
        _camera?.CloseFramegrabber();
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

        if (hv_Image is not HImage himage)
            return;

        Matrix matrix = _convertHImageToMatrix(himage);

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
            Data = imageData
        };

        return matrix;
    }
}
