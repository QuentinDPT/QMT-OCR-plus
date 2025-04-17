using Microsoft.AspNetCore.Mvc;
using QMTGroup.Web.Service;

namespace QMTGroup.Web.Controller;

[Route("api/[controller]")]
[ApiController]
public class OverlayController : ControllerBase
{
    private readonly OverlayService _overlayService;

    public OverlayController(OverlayService overlayService)
    {
        _overlayService = overlayService;
    }

    [HttpGet]
    public IActionResult FromDxf(string dxfName = "input")
    {
        var svgBytes = _overlayService.SvgFromDxf(dxfName);

        return Content(svgBytes, "image/svg+xml", System.Text.Encoding.UTF8);
    }
}
