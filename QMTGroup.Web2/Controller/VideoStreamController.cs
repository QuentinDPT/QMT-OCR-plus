using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.AspNetCore.Mvc;
using QMTGroup.Image;
using QMTGroup.Web.Service;
using SixLabors.ImageSharp;
using QMTGroup.Image.Interface;

namespace QMTGroup.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoStreamController : ControllerBase
    {
        private VideoStreamService _videoStreamService;
        private readonly IJpegConverter _jpegConverter;
        private ILogger _logger;

        public VideoStreamController(IJpegConverter jpegConverter, ILogger<VideoStreamController> logger, VideoStreamService videoStreamService)
        {
            _videoStreamService = videoStreamService;
            _jpegConverter = jpegConverter;
            _logger = logger;
        }

        [HttpGet("apply/{filterId}")]
        public IActionResult ApplyFilter(int filterId, [FromQuery] string? parameter1 = null, [FromQuery] string? parameter2 = null)
        {
            switch (filterId)
            {
                case 0:
                    _videoStreamService.ImageFilter = null;
                    return Ok();
                case 1:
                    //_videoStreamService.ImageFilter = new ToGrayScales();
                    return Ok();
                case 2:
                    if (int.TryParse(parameter1, out int size))
                    {
                        float sigma;
                        if (!float.TryParse(parameter2, out sigma))
                            sigma = 1.5f;

                        //_videoStreamService.ImageFilter = new ConvolutionGauss(size, sigma);
                    }
                    else
                    {
                        //_videoStreamService.ImageFilter = new ConvolutionGauss();
                    }

                    return Ok();
            }
            return Problem();
        }

        [HttpGet("/mjpeg-stream")]
        public async Task<IActionResult> MjpegStream_2([FromQuery] Guid cameraInstance)
        {
            int imageWaitTimeout = 200;
            Matrix img = new Matrix(System.Buffers.ArrayPool<byte>.Create(1 << 30, 1));
            byte[] imageDataHeader = [];

            Response.ContentType = "multipart/x-mixed-replace; boundary=frame";

            do
            {
                if(!_videoStreamService.ImageHasChanged.WaitOne(imageWaitTimeout))
                    continue;

                if (_videoStreamService.LastImage is null)
                    continue;

                if(_videoStreamService.LastImage.Data.Length != img.Data.Length)
                {
                    img.SetDataSize(_videoStreamService.LastImage.Data.Length);
                    imageDataHeader = System.Text.Encoding.UTF8.GetBytes($"--frame\r\nContent-Type: image/jpeg\r\nContent-Length: {img.Data.Length}\r\n\r\n");
                }
                img.SetData(_videoStreamService.LastImage.Data);

                _videoStreamService.ImageHasChanged.Reset();

                // Envoi de l'image dans le format MJPEG
                await Response.Body.WriteAsync(imageDataHeader);
                await Response.Body.WriteAsync(_jpegConverter.ConvertToJpeg(img, 1));
                await Response.Body.WriteAsync([13, 10], 0, 2);
                await Response.Body.FlushAsync();
            }
            while (!HttpContext.RequestAborted.IsCancellationRequested);

            img.Dispose();

            return new EmptyResult();
        }

        public async Task<IActionResult> MjpegStream([FromQuery] Guid cameraInstance)
        {
            Response.ContentType = "multipart/x-mixed-replace; boundary=frame";
            Matrix? img = new();
            byte[] imageBytes;
            Mat er = new Mat(@"C:\Users\quentin.de-potter\Dev\Q027\resource\image\mecanism1.png");
            _videoStreamService.ImageHasChanged.WaitOne();
            img = _videoStreamService.LastImage;
            _videoStreamService.ImageHasChanged.Reset();
            if (img is null)
                return new EmptyResult();
            var imageDataHeader = System.Text.Encoding.UTF8.GetBytes($"--frame\r\nContent-Type: image/jpeg\r\nContent-Length: {img.Data.Length}\r\n\r\n");

            // Diffuser les images reçues par l'événement ImageCaptured
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                // Attendre que de nouvelles images arrivent
                _videoStreamService.ImageHasChanged.WaitOne();
                img = _videoStreamService.LastImage;
                _videoStreamService.ImageHasChanged.Reset();

                //if (img is null) continue;

                if (img.Channels == 1)
                {
                    imageBytes = er.ToImage<Gray, byte>().ToJpegData(95);
                }
                else
                {
                    imageBytes = er.ToImage<Bgr, byte>().ToJpegData(95);
                }


                // Envoi de l'image dans le format MJPEG
                await Response.Body.WriteAsync(imageDataHeader);
                await Response.Body.WriteAsync(imageBytes);
                await Response.Body.WriteAsync([13, 10], 0, 2);
                await Response.Body.FlushAsync();
            }
            return new EmptyResult();
        }

        [HttpPost("start")]
        public IActionResult StartCamera([FromQuery] Guid cameraInstance)
        {
            try
            {
                _videoStreamService.Start(cameraInstance);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Something went wrong with the camera start.");
                return Problem("Something went wrong with the camera start.");
            }
            return Ok();
        }


        [HttpPost("stop")]
        public IActionResult StopCamera([FromQuery] Guid cameraInstance)
        {
            try
            {
                _videoStreamService.Stop(cameraInstance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong with the camera stop.");
                return Problem("Something went wrong with the camera stop.");
            }
            return Ok();
        }

        [HttpGet("allCamera")]
        public IActionResult AllCamera()
        {
            return new JsonResult(_videoStreamService.GetAllCamera());
        }
    }
}
