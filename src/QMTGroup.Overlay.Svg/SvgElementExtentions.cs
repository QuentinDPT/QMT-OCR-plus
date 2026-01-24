using Svg;
using Svg.Transforms;

namespace QMTGroup.Overlay.Svg;

public static class SvgElementExtentions
{
    public static SvgDocument ToDocument(this SvgVisualElement element)
    {
        float hTarget = 400;
        float w = hTarget/ element.Bounds.Width * element.Bounds.Height;
        var doc = new SvgDocument()
        {
            Width = w,
            Height = hTarget,
            ViewBox = new SvgViewBox(element.Bounds.X, element.Bounds.Y, element.Bounds.Width, element.Bounds.Height),
        };
        doc.Children.Add(element);
        return doc;
    }
}
