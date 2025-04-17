using QMTGroup.Overlay.Dxf;
using QMTGroup.Overlay.Svg;
using Svg;
using System.Drawing;
using System.Text;
using System.Xml;

namespace QMTGroup.Web.Service;

public class OverlayService
{
    private const string _configurationKey = "Services:Overlay:DxfFolder";

    private readonly string _documentSource;

    public OverlayService(IConfiguration configuration)
    {
        string docSource = configuration.GetValue<string>(_configurationKey) ?? throw new KeyNotFoundException($"Dxf source document was not found under the key \"{_configurationKey}\"");

        if (!Directory.Exists(docSource))
            throw new DirectoryNotFoundException();

        _documentSource = docSource;
    }


    public string SvgFromDxf(string dxfName)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        };

        string dxfPath = _getFilePath(dxfName);

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


    private string _getFilePath(string fileName)
    {
        if (!fileName.EndsWith(".dxf"))
            fileName += ".dxf";

        string filePath = Path.Combine(_documentSource, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException();

        return filePath;
    }
}
