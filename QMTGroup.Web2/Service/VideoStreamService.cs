using Microsoft.AspNetCore.SignalR;
using QMTGroup.Camera;
using QMTGroup.Image;
using QMTGroup.ImageFilters;

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
        _camera.OnReciveImage += _camera_OnReciveImage_mjpeg;
        _camera.StartCapture();
    }

    public ManualResetEvent ImageHasChanged = new ManualResetEvent(false);

    public Matrix? LastImage;

    private void _camera_OnReciveImage_mjpeg(object? sender, Image.Matrix e)
    {
        LastImage = e;
        ImageHasChanged.Set();
    }

    public void Stop()
    {
        _camera.StopCapture();
    }
}
