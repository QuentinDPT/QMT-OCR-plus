using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.AspNetCore.Mvc;
using QMTGroup.Image;
using QMTGroup.Web.Service;
using QMTGroup.Urn;
using System.Text.Json;

namespace QMTGroup.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoStreamController : ControllerBase
    {
        private VideoStreamService _videoStreamService;
        private ILogger _logger;

        public VideoStreamController(ILogger<VideoStreamController> logger, VideoStreamService videoStreamService)
        {
            _videoStreamService = videoStreamService;
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
        public async Task<IActionResult> MjpegStream([FromQuery] Guid cameraInstance)
        {
            Response.ContentType = "multipart/x-mixed-replace; boundary=frame";
            Matrix? img = new();
            byte[] imageBytes;
            Mat er;

            // Diffuser les images reçues par l'événement ImageCaptured
            while (true)
            {
                // Attendre que de nouvelles images arrivent
                _videoStreamService.ImageHasChanged.WaitOne();
                img = _videoStreamService.LastImage;
                _videoStreamService.ImageHasChanged.Reset();

                if (img is null) continue;

                er = img.ToMat();

                if (img.Channels == 1)
                {
                    imageBytes = er.ToImage<Gray, byte>().ToJpegData(95);
                }
                else
                {
                    imageBytes = er.ToImage<Bgr, byte>().ToJpegData(95);
                }


                // Envoi de l'image dans le format MJPEG
                await Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes($"--frame\r\nContent-Type: image/jpeg\r\nContent-Length: {img.Data.Length}\r\n\r\n"));
                await Response.Body.WriteAsync(imageBytes);
                await Response.Body.WriteAsync([13, 10], 0, 2);
                await Response.Body.FlushAsync();
            }
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

        [HttpGet("camera")]
        public IActionResult CameraParameters([FromQuery] Guid cameraInstance)
        {
            return new JsonResult(_videoStreamService.GetCamera(cameraInstance), new JsonSerializerOptions
            {
                Converters = { new UrnConverter() }
            });
        }
    }
}
