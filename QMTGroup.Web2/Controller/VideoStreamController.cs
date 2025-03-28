using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.AspNetCore.Mvc;
using QMTGroup.Image;
using QMTGroup.Web2.Service;

namespace QMTGroup.Web2.Controller
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
        public async Task<IActionResult> MjpegStream()
        {
            Response.ContentType = "multipart/x-mixed-replace; boundary=frame";
            Matrix img = new();

            // Diffuser les images reçues par l'événement ImageCaptured
            while (true)
            {
                // Attendre que de nouvelles images arrivent
                _videoStreamService.ImageHasChanged.WaitOne();
                var nimg = _videoStreamService.LastImage;
                _videoStreamService.ImageHasChanged.Reset();

                if (nimg is not null && nimg != img)
                {
                    img = nimg;


                    // Convertir Mat en Image<Bgr, byte>
                    var er = img.ToMat();

                    // Encoder en JPEG
                    byte[] imageBytes;

                    if (img.Channels == 1)
                    {
                        var image = er.ToImage<Gray, byte>();
                        imageBytes = image.ToJpegData();
                    }
                    else
                    {
                        Image<Bgr, byte> image = er.ToImage<Bgr, byte>();
                        imageBytes = image.ToJpegData();
                    }


                    // Envoi de l'image dans le format MJPEG
                    await Response.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes($"--frame\r\nContent-Type: image/jpeg\r\nContent-Length: {_videoStreamService.LastImage.Data.Length}\r\n\r\n"));
                    await Response.Body.WriteAsync(imageBytes);
                    await Response.Body.WriteAsync([13, 10], 0, 2);
                    await Response.Body.FlushAsync();
                }
            }
        }

        [HttpPost("start")]
        public IActionResult StartCamera()
        {
            try
            {
                _videoStreamService.Start();
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Something went wrong with the camera start.");
                return Problem("Something went wrong with the camera start.");
            }
            return Ok();
        }


        [HttpPost("stop")]
        public IActionResult StopCamera()
        {
            try
            {
                _videoStreamService.Stop();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong with the camera stop.");
                return Problem("Something went wrong with the camera stop.");
            }
            return Ok();
        }
    }
}
