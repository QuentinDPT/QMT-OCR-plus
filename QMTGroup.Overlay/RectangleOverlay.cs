using Svg;

namespace QMTGroup.Overlay;

public sealed class RectangleOverlay : IOverlay
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public SvgGroup ToSvg()
    {
        throw new NotImplementedException();
    }
}
