using Microsoft.AspNetCore.Mvc;

namespace QMTGroup.Web.Controller;

[Route("api/[controller]")]
[ApiController]
public class OverlayController : ControllerBase
{
    [HttpGet]
    public IActionResult FromDxf(string dxfPath = @"C:\Users\quentin.de-potter\Dev\PERSO\QMT-OCR-plus\QMTGroup.Web2\wwwroot\img\input.dxf")
    {
        var svgBytes = new Service.OverlayService().SvgFromDxf(dxfPath);

        return Content(svgBytes, "image/svg+xml", System.Text.Encoding.UTF8);
    }
}
