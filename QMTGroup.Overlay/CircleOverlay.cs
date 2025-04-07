using Svg;

namespace QMTGroup.Overlay;

public sealed class CricleOverlay : IOverlay
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Radius { get; set; }

    public SvgGroup ToSvg()
    {
        throw new NotImplementedException();
    }
}
