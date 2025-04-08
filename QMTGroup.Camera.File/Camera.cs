using QMTGroup.Image;
using System.Drawing;

namespace QMTGroup.Camera.File;

public class Camera : ICamera
{
    private readonly CameraParameters _parameters;
    private Matrix _image;
    private Task _acquisitionTask = Task.CompletedTask;
    private CancellationTokenSource _cancellationToken = new();

    public Camera(CameraParameters parameters)
    {
        _parameters = parameters;

        if (!System.IO.File.Exists(parameters.Path))
        {
            
        }
    }

    /// <inheritdoc/>
    public event EventHandler<Matrix> OnReciveImage;

    /// <inheritdoc/>
    public void StartCapture()
    {
        if (!_acquisitionTask.IsCompleted)
            return;

        _image = _loadImage(_parameters.Path);

        _acquisitionTask = Task.Run(() => _capturePeriodically(_cancellationToken.Token));
    }

    private Matrix _loadImage(string resourcePath)
    {
        Matrix result;
        using (var image = new Bitmap(resourcePath))
        {
            result = new Matrix(image.Width, image.Height, DataType.BGR_8);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    System.Drawing.Color pixelColor = image.GetPixel(x, y);
                    result.SetPixel(x, y, [pixelColor.B, pixelColor.G, pixelColor.R]);  // Par exemple, on peut juste stocker la composante rouge
                }
            }
        }
        return result;
    }

    private static DataType _getDataType(System.Drawing.Imaging.PixelFormat format)
    {
        switch (format)
        {
            case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                return DataType.XRGB_8;
            default:
                throw new NotImplementedException();
        }
    }

    private void _capturePeriodically(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            OnReciveImage?.Invoke(null, _image);
            Thread.Sleep(_parameters.AcquisitionLoopSleep);
        }
    }

    /// <inheritdoc/>
    public void StopCapture()
    {
        _cancellationToken.Cancel();
        _acquisitionTask?.Wait();
        _cancellationToken = new CancellationTokenSource();
        _image.Dispose();
    }
}
