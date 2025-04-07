using QMTGroup.Image;
using System.Drawing;
using System.Threading;

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
            int width = image.Width;
            int height = image.Height;
            result = new Matrix(width, height, 3)
            {
                ChannelType = typeof(byte)
            };

            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    System.Drawing.Color pixelColor = image.GetPixel((int)x, (int)y);
                    result.SetPixel(x, y, [pixelColor.B, pixelColor.G, pixelColor.R]);  // Par exemple, on peut juste stocker la composante rouge
                }
            }
        }

        return result;
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
