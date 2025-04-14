using QMTGroup.Overlay.Dxf;
using QMTGroup.Overlay.Svg;
using Svg;
using System.Drawing;
using System.Text;
using System.Xml;

namespace QMTGroup.Web.Service;

public class OverlayService
{
    public string SvgFromDxf(string dxfPath)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        };

        var dxf = new DxfOverlay(dxfPath);
        var svgGroup = dxf.ToSvg(
            new()
            {
                {"vector-effect", "non-scaling-stroke"}
            });
        svgGroup.Fill = SvgPaintServer.None;
        svgGroup.Stroke = new SvgColourServer(Color.Blue);
        svgGroup.StrokeWidth = new SvgUnit(SvgUnitType.Pixel, 1);
        var svg = svgGroup.ToDocument();

        using(var sw = new StringWriter())
        using (var stringWriter = XmlWriter.Create(sw, settings))
        {
            svg.Write(stringWriter);
            stringWriter.Flush();
            return sw.ToString() ?? string.Empty;
        }
    }
}
