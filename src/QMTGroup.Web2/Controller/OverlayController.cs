using Microsoft.AspNetCore.Mvc;
using QMTGroup.Web.Service;
using System.Runtime.CompilerServices;

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

    [HttpGet("/overlay-stream")]
    public async Task OverlayStream()
    {
        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        Random random = new();

        int counter = 0;

        while (!HttpContext.RequestAborted.IsCancellationRequested)
        {
            await Task.Delay(20);

            var overlay = new OverlayFrame
            {
                Shapes =
                [
                    // Rectangle orientable
                    new RectangleShape
                    {
                        Id = "rect_1",
                        X = 400,
                        Y = 250,
                        Width = 220,
                        Height = 120,
                        Rotation = counter,
                        Stroke = "#FF0000FF",
                        Fill = "#FF000040",
                        StrokeWidth = 1
                    },

                    // Cercle
                    new CircleShape
                    {
                        Id = "circle_1",
                        Cx = 700,
                        Cy = 300,
                        Radius = 80,
                        Stroke = "#00FF00FF",
                        Fill = "#00FF0040",
                        StrokeWidth = 1
                    },

                    // Ligne
                    new LineShape
                    {
                        Id = "line_1",
                        X1 = 100,
                        Y1 = 100,
                        X2 = 900,
                        Y2 = 500,
                        Stroke = "#FFFFFFFF",
                        StrokeWidth = 1
                    }
                ]
            };

            if (Random.Shared.Next(0, 100) > 800)
            {
                overlay.Shapes.Add(new RectangleShape
                {
                    Id = "rect_2",
                    X = 200,
                    Y = 250,
                    Width = 220,
                    Height = 120,
                    Rotation = random.NextDouble() * 360.0,
                    Stroke = "#FF0000FF",
                    Fill = "#00000000",
                    StrokeWidth = 1
                });
            }

            if (Random.Shared.Next(0, 100) > 800)
            {
                overlay.Shapes.Add(new CircleShape
                {
                    Id = "circle_2",
                    Cx = 300,
                    Cy = 400,
                    Radius = 50,
                    Stroke = "#00FF00FF",
                    Fill = "#00FF0000",
                    StrokeWidth = 2
                });
            }

            string json = overlay.ToJson();

            await Response.WriteAsync($"data: {json}\n\n");
            await Response.Body.FlushAsync();

            counter++;
        }
    }
}

#region Models

public class OverlayFrame
{
    public List<IShape> Shapes { get; set; } = [];

    public string ToJson()
    {
        string result = "[";

        foreach(var s in Shapes)
        {
            result += s.ToJson() + ",";
        }

        result = result.Substring(0, result.Length -1) + "]";

        return result;
    }
}

public interface IShape
{
    string Type { get; }
    string Id { get; set; }

    string ToJson();
}

public class RectangleShape : IShape
{
    public string Type => "rect";

    public string Id { get; set; } = "";

    public double X { get; set; }
    public double Y { get; set; }

    public double Width { get; set; }
    public double Height { get; set; }

    // Rotation en degrés
    public double Rotation { get; set; }

    public string Stroke { get; set; } = "#FFFFFFFF";
    public string Fill { get; set; } = "#00000000";

    public double StrokeWidth { get; set; } = 1;

    public string ToJson()
    {
        var inv = System.Globalization.CultureInfo.InvariantCulture;
        return
            "{\"type\":\"" + Type + "\"," +
            "\"id\":\"" + Id.Replace("\"", "\\\"") + "\"," +
            "\"x\":" + X.ToString(inv) + "," +
            "\"y\":" + Y.ToString(inv) + "," +
            "\"width\":" + Width.ToString(inv) + "," +
            "\"height\":" + Height.ToString(inv) + "," +
            "\"rotation\":" + Rotation.ToString(inv) + "," +
            "\"stroke\":\"" + Stroke.Replace("\"", "\\\"") + "\"," +
            "\"strokeWidth\":" + StrokeWidth.ToString(inv) + "," +
            "\"fill\":\"" + Fill.Replace("\"", "\\\"") + "\"}";
    }
}

public class CircleShape : IShape
{
    public string Type => "circle";

    public string Id { get; set; } = "";

    public double Cx { get; set; }
    public double Cy { get; set; }

    public double Radius { get; set; }

    public string Stroke { get; set; } = "#FFFFFFFF";
    public string Fill { get; set; } = "#00000000";

    public double StrokeWidth { get; set; } = 1;

    public string ToJson()
    {
        var inv = System.Globalization.CultureInfo.InvariantCulture;
        return
            "{\"type\":\"" + Type + "\"," +
            "\"id\":\"" + Id.Replace("\"", "\\\"") + "\"," +
            "\"cx\":" + Cx.ToString(inv) + "," +
            "\"cy\":" + Cy.ToString(inv) + "," +
            "\"radius\":" + Radius.ToString(inv) + "," +
            "\"stroke\":\"" + Stroke.Replace("\"", "\\\"") + "\"," +
            "\"strokeWidth\":" + StrokeWidth.ToString(inv) + "," +
            "\"fill\":\"" + Fill.Replace("\"", "\\\"") + "\"}";
    }
}

public class LineShape : IShape
{
    public string Type => "line";

    public string Id { get; set; } = "";

    public double X1 { get; set; }
    public double Y1 { get; set; }

    public double X2 { get; set; }
    public double Y2 { get; set; }

    public string Stroke { get; set; } = "#FFFFFFFF";

    public double StrokeWidth { get; set; } = 1;

    public string ToJson()
    {
        var inv = System.Globalization.CultureInfo.InvariantCulture;
        return
            "{\"type\":\"" + Type + "\"," +
            "\"id\":\"" + Id.Replace("\"", "\\\"") + "\"," +
            "\"x1\":" + X1.ToString(inv) + "," +
            "\"y1\":" + Y1.ToString(inv) + "," +
            "\"x2\":" + X2.ToString(inv) + "," +
            "\"y2\":" + Y2.ToString(inv) + "," +
            "\"stroke\":\"" + Stroke.Replace("\"", "\\\"") + "\"," +
            "\"strokeWidth\":" + StrokeWidth.ToString(inv) +
            "}";
    }
}

#endregion

