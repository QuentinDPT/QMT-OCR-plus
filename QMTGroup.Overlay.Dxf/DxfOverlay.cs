using netDxf;
using netDxf.Entities;

namespace QMTGroup.Overlay.Dxf;

public class DxfOverlay : GroupOverlay
{
    public DxfOverlay(string dxfLocation)
    {
        DxfDocument dxf = DxfDocument.Load(dxfLocation);

        FromDxf(dxf);
    }

    internal void FromDxf(DxfDocument dxf)
    {
        foreach (EntityObject? entity in dxf.Entities.All)
        {
            IOverlay layer;

            switch (entity)
            {
                case Line line:
                    layer = new LineOverlay()
                    {
                        X1 = line.StartPoint.X,
                        Y1 = line.StartPoint.Y,
                        X2 = line.EndPoint.X,
                        Y2 = line.EndPoint.Y,
                    };
                    break;
                case Circle circle:
                    layer = new CircleOverlay()
                    {
                        X = circle.Center.X,
                        Y = circle.Center.Y,
                        Radius = circle.Radius,
                    };
                    break;
                case Arc arc:
                    layer = new ArcOverlay()
                    {
                        X = arc.Center.X,
                        Y = arc.Center.Y,
                        Radius = arc.Radius,
                        StartAngle = arc.StartAngle,
                        EndAngle = arc.EndAngle,
                    };
                    break;
                case Polyline2D polyline:
                    layer = new PolygonOverlay()
                    {
                        Vertices = polyline.Vertexes.Select(x => (x.Position.X, x.Position.Y)).ToList(),
                    };
                    break;
                default:
                    continue;
            }

            Add(layer);
        }
    }
}
