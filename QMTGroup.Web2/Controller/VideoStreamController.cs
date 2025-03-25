using LuaOrchestrator;
using Microsoft.AspNetCore.Mvc;
using QMTGroup.Image;
using QMTGroup.Web2.Service;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace QMTGroup.Web2.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoStreamController : ControllerBase
    {
        private VideoStreamService _videoStreamService;

        public VideoStreamController(VideoStreamService videoStreamService)
        {
            _videoStreamService = videoStreamService;
        }

        [HttpGet("{id}")]
        public IActionResult ee(string id)
        {
            var image = QMT.Bank[id] as Matrix;
            return File(ConvertMatToBase64(image), "image/jpeg");
        }

        private byte[] ConvertMatToBase64(Matrix mat) => Convert.ToBase64String(mat.Data).Select(x => (byte)x).ToArray();

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
    }
}
