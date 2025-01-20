using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.SignalR;
using QMTGroup.IO.Camera;
using QMTGroup.Models.ImageFilters;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace QMTGroup.Web2.Service
{
    public class VideoStreamService
    {
        private readonly IHubContext<VideoHub> _hubContext;
        private readonly VideoCapture _capture;
        private readonly CancellationTokenSource _cts = new();

        public IImageFilter? ImageFilter
        {
            get => _imageFilter;
            set => _imageFilter = value;
        }

        private IImageFilter? _imageFilter = null;

        public VideoStreamService(IHubContext<VideoHub> hubContext)
        {
            _hubContext = hubContext;
            _capture = new USBCamera(0);
            _capture.Set(CapProp.FrameWidth, 1600);
            _capture.Set(CapProp.FrameHeight, 900);
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var frame = new Mat();
                    _capture.Read(frame);
                    if (!frame.IsEmpty)
                    {
                        if(_imageFilter != null)
                        {
                            frame = _imageFilter.ApplyFilter(frame);
                        }

                        var base64Image = ConvertMatToBase64(frame);
                        await _hubContext.Clients.All.SendAsync("ReceiveFrame", base64Image);
                    }
                    await Task.Delay(1); // ~30 FPS
                }
            });
        }

        public void Stop()
        {
            _cts.Cancel();
            _capture.Dispose();
        }

        private string ConvertMatToBase64(Mat mat)
        {
            using var ms = new MemoryStream();
            using var image = Image.LoadPixelData<Bgr24>(mat.ToImage<Bgr, byte>().Bytes, mat.Width, mat.Height);
            image.Save(ms, new JpegEncoder());
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
