using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.SignalR;
using QMTGroup.Camera;
using QMTGroup.ImageFilters;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace QMTGroup.Web2.Service;

public class VideoStreamService
{
    private readonly IHubContext<VideoHub> _hubContext;
    private readonly ICamera _camera;
    
    public IImageFilter? ImageFilter
    {
        get => _imageFilter;
        set => _imageFilter = value;
    }

    private IImageFilter? _imageFilter = null;

    public VideoStreamService(IHubContext<VideoHub> hubContext, ICamera camera)
    {
        _hubContext = hubContext;
        _camera = camera;
    }

    public void Start()
    {
        _camera.OnReciveImage += _camera_OnReciveImage;
        _camera.StartCapture();
    }

    private void _camera_OnReciveImage(object? sender, Image.Matrix e)
    {
        var base64Image = ConvertMatToBase64(e);
        _hubContext.Clients.All.SendAsync("ReceiveFrame", base64Image).Wait();
    }

    public void Stop()
    {
        _camera.StopCapture();
    }

    private static string ConvertMatToBase64(Image.Matrix matrix)
    {
        Mat mat = new Mat((int)matrix.Height, (int)matrix.Width, DepthType.Cv8U, (int)matrix.Channels);
        mat.SetTo(matrix.Data);

        return ConvertMatToBase64(mat);
    }

    private static string ConvertMatToBase64(Mat mat)
    {
        using var ms = new MemoryStream();

        byte[] imageBytes = mat.ToImage<Bgr, byte>().Bytes;
        ReadOnlySpan<Bgr24> pixelData = MemoryMarshal.Cast<byte, Bgr24>(imageBytes);

        Image<Bgr24> image = SixLabors.ImageSharp.Image.LoadPixelData<Bgr24>(pixelData, mat.Width, mat.Height);
        image.Save(ms, new JpegEncoder());
        return Convert.ToBase64String(ms.ToArray());
    }
}
