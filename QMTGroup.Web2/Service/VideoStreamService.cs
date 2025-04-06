using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QMTGroup.Camera;
using QMTGroup.Image;
using QMTGroup.ImageFilters;
using QMTGroup.Web.Factory;

namespace QMTGroup.Web.Service;

public class VideoStreamService
{
    private readonly ICameraFactory _cameraFactory;
    
    public IImageFilter? ImageFilter
    {
        get => _imageFilter;
        set => _imageFilter = value;
    }

    private IImageFilter? _imageFilter = null;

    public VideoStreamService(ICameraFactory cameraFactory)
    {
        _cameraFactory = cameraFactory;
    }

    public void Start(Guid cameraInstance)
    {
        var camera = _cameraFactory[cameraInstance];
        if (camera is null)
            return;

        camera.OnReciveImage += _camera_OnReciveImage_mjpeg;
        camera.StartCapture();
    }

    public ManualResetEvent ImageHasChanged = new ManualResetEvent(false);

    public Matrix? LastImage;

    private void _camera_OnReciveImage_mjpeg(object? sender, Image.Matrix e)
    {
        LastImage = e;
        ImageHasChanged.Set();
    }

    public void Stop(Guid cameraInstance)
    {
        var camera = _cameraFactory[cameraInstance];
        if (camera is null)
            return;

        camera.StopCapture();
    }

    internal Dictionary<Guid, string> GetAllCamera()
    {
        return _cameraFactory.ToDictionary(x => x.Key, x => x.Value.GetType().FullName ?? x.Value.GetType().Name);
    }

    internal ICamera GetCamera(Guid cameraInstance)
    {
        return _cameraFactory.First(x => x.Key == cameraInstance).Value;
    }
}
