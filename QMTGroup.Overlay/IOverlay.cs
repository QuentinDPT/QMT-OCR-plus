using Svg;

namespace QMTGroup.Overlay;

public interface IOverlay
{
    /// <summary>
    /// Converts the present overlay to a <c>svg</c>.
    /// </summary>
    /// <returns>The corresponding <c>svg</c>.</returns>
    public SvgGroup ToSvg();
}
