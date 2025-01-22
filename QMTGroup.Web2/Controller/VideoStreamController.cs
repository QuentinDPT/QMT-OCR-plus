using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QMTGroup.ImageFilters.Filters;
using QMTGroup.Web2.Service;

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

        [HttpGet("apply")]
        public IActionResult ee()
        {
            return Ok();
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
                    _videoStreamService.ImageFilter = new ToGrayScales();
                    return Ok();
                case 2:
                    if (int.TryParse(parameter1, out int size))
                    {
                        float sigma;
                        if (!float.TryParse(parameter2, out sigma))
                            sigma = 1.5f;

                        _videoStreamService.ImageFilter = new ConvolutionGauss(size, sigma);
                    }
                    else
                    {
                        _videoStreamService.ImageFilter = new ConvolutionGauss();
                    }

                    return Ok();
            }
            return Problem();
        }
    }
}
