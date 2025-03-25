using Microsoft.AspNetCore.SignalR;
using QMTGroup.Camera;
using QMTGroup.Models.ImageFilters;
using QMTGroup.Web2;

namespace QMTGroup.Service;

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

    private string ConvertMatToBase64(Image.Matrix mat) => Convert.ToBase64String(mat.Data);
}
