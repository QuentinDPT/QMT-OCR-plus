namespace QMTGroup.Overlay;

public class PolygonOverlay: IOverlay
{
    public List<(double X, double Y)> Vertices { get; set; } = new List<(double X, double Y)>();
}
