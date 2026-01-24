using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ImagePresenter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        private readonly MemoryStreamService _streamService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, MemoryStreamService streamService)
        {
            _logger = logger;
            _streamService = streamService;
        }


        [HttpGet("memory-stream")]
        public async Task<IActionResult> MjpegStream_2()
        {
            Response.ContentType = "text/plain";
            Response.Headers.Add("Cache-Control", "no-cache");

            do
            {
                string message = await _streamService.GetStreamAsync();
                await Response.WriteAsync(message + "\n");
                await Response.Body.FlushAsync(); // envoie immédiatement
                await Task.Delay(50);
            }
            while (!HttpContext.RequestAborted.IsCancellationRequested);

            return new EmptyResult();
        }

        [HttpGet("start-process")]
        public IActionResult StartProcess()
        {
            if (_streamService.StartProcess())
                return Ok();
            return Problem();
        }

        [HttpGet("stop-process")]
        public IActionResult StopProcess()
        {
            if (_streamService.StopProcess())
                return Ok();
            return Problem();
        }
    }
}
