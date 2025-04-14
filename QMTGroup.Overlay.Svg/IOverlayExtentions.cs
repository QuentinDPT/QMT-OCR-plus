using Svg;
using Svg.Pathing;
using System.Drawing;

namespace QMTGroup.Overlay.Svg;

public static class IOverlayExtentions
{
    public static SvgVisualElement ToSvg(this IOverlay overlay, Dictionary<string, string>? customAttributes = null)
    {
        customAttributes ??= new Dictionary<string, string>();

        SvgVisualElement element;

        switch (overlay)
        {
            case CircleOverlay circle:
                element = _circleToSvg(circle, customAttributes);
                break;
            case LineOverlay line:
                element = _lineToSvg(line, customAttributes);
                break;
            case ArcOverlay arc:
                element = _arcToSVG(arc, customAttributes);
                break;
            case GroupOverlay group:
                element = _groupToSvg(group, customAttributes);
                break;
            case RectangleOverlay rect:
                element = _rectToSvg(rect, customAttributes);
                break;
            case PolygonOverlay polygon:
                element = _polygonToSvg(polygon, customAttributes);
                break;
            default:
                throw new NotImplementedException();
        }

        foreach (var attr in customAttributes)
        {
            element.CustomAttributes[attr.Key] = attr.Value;
        }

        return element;
    }

    private static SvgVisualElement _polygonToSvg(PolygonOverlay polygon, Dictionary<string, string> customAttributes)
    {
        if (polygon.Vertices.Count < 2)
            throw new ArgumentException("Il faut au moins 2 points pour créer un chemin.");

        SvgPathSegmentList pathData =
        [
            new SvgMoveToSegment(new PointF((float)polygon.Vertices.First().X, (float)polygon.Vertices.First().Y)),
        ];

        foreach (var vertex in polygon.Vertices)
        {
            pathData.Add(new SvgLineSegment(false, new PointF((float)vertex.X, (float)vertex.Y)));
        }

        return new SvgPath
        {
            PathData = pathData
        };
    }

#warning a approffondir
    private static SvgVisualElement _arcToSVG(ArcOverlay arc, Dictionary<string, string> customAttributes)
    {
        double xStart = arc.X + arc.Radius * (float)Math.Cos(arc.StartAngle);
        double yStart = arc.Y + arc.Radius * (float)Math.Sin(arc.EndAngle);

        var pathData = new SvgPathSegmentList
        {
            new SvgMoveToSegment(new System.Drawing.PointF((float)xStart, (float)yStart))
        };

        return new SvgPath() { PathData = pathData };
    }

    private static SvgVisualElement _lineToSvg(LineOverlay line, Dictionary<string, string> customAttributes)
    {
        return new SvgLine()
        {
            StartX = (float)line.X1,
            StartY = (float)line.Y1,
            EndX = (float)line.X2,
            EndY = (float)line.Y2,
        };
    }

    private static SvgVisualElement _circleToSvg(CircleOverlay circle, Dictionary<string, string> customAttributes)
    {
        return new SvgCircle()
        {
            CenterX = (float)circle.X,
            CenterY = (float)circle.Y,
            Radius = (float)circle.Radius,
        };
    }

    private static SvgVisualElement _groupToSvg(GroupOverlay group, Dictionary<string, string> customAttributes)
    {
        SvgGroup svgGroup = new();

        foreach (var item in group)
        {
            svgGroup.Children.Add(item.ToSvg(customAttributes));
        }

        return svgGroup;
    }

    private static SvgVisualElement _rectToSvg(RectangleOverlay rect, Dictionary<string, string> customAttributes)
    {
        return new SvgRectangle()
        {
            X = (float)rect.X,
            Y = (float)rect.Y,
            Width = (float)rect.Width,
            Height = (float)rect.Height,
        };
    }
}
