namespace QMTGroup.DSL.Library.Math;

public partial class MathLib
{
    [DSLFunction]
    public double RadToDeg(double radian)
    {
        return radian * (180.0 / System.Math.PI);
    }

    [DSLFunction]
    public double DegToRad(double degres)
    {
        return degres * (System.Math.PI / 180.0);
    }
}
