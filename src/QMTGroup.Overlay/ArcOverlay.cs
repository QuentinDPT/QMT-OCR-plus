namespace QMTGroup.Overlay;

public class ArcOverlay : IOverlay
{
    public double X { get; set; }

    public double Y { get; set; }

    public double Radius { get; set; }

    /// <summary>
    /// In degres.
    /// </summary>
    public double StartAngle { get; set; }

    /// <summary>
    /// In degres.
    /// </summary>
    public double EndAngle { get; set; }
}
