using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ImagePresenter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AcquisitionDispatcherController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        private readonly ImageStreamService _imageStreamService;

        public AcquisitionDispatcherController(ILogger<WeatherForecastController> logger, ImageStreamService imageStreamService)
        {
            _logger = logger;
            _imageStreamService = imageStreamService;
        }




        [HttpGet("start")]
        public IActionResult Start()
        {
            _imageStreamService.Start();
            return Ok();
        }


        [HttpGet("stop")]
        public IActionResult Stop()
        {
            _imageStreamService.Stop();
            return Ok();
        }


        [HttpGet("stream-single-read")]
        public IActionResult Stream()
        {
            string base64String = _imageStreamService.Read().Split("base64,").Last();

            return File(Convert.FromBase64String(base64String), "image/png");
        }
    }
}
