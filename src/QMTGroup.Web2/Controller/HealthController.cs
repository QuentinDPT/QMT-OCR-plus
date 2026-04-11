using Microsoft.AspNetCore.Mvc;

namespace QMTGroup.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {

        [HttpGet("check")]
        public IActionResult Check() => Ok();
    }
}
