namespace QMTGroup.DSL.Library.Math;

[DSLNamespace("Math")]
public partial class MathLib : IDSLLibrary
{
    [DSLConstant]
    public double E => System.Math.E;

    [DSLConstant]
    public double PI => System.Math.PI;

    #region Abs

    [DSLFunction]
    public int Abs(int x) => System.Math.Abs(x);

    [DSLFunction]
    public float Abs(float x) => System.Math.Abs(x);

    [DSLFunction]
    public short Abs(short x) => System.Math.Abs(x);

    [DSLFunction]
    public long Abs(long x) => System.Math.Abs(x);

    [DSLFunction]
    public nint Abs(nint x) => System.Math.Abs(x);

    [DSLFunction]
    public sbyte Abs(sbyte x) => System.Math.Abs(x);

    [DSLFunction]
    public decimal Abs(decimal x) => System.Math.Abs(x);

    [DSLFunction]
    public double Abs(double x) => System.Math.Abs(x);

    #endregion // Abs

    #region Max

    [DSLFunction]
    public int Max(int x1, int x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public float Max(float x1, float x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public short Max(short x1, short x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public long Max(long x1, long x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public nint Max(nint x1, nint x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public sbyte Max(sbyte x1, sbyte x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public decimal Max(decimal x1, decimal x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public double Max(double x1, double x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public byte Max(byte x1, byte x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public ushort Max(ushort x1, ushort x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public uint Max(uint x1, uint x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public ulong Max(ulong x1, ulong x2) => System.Math.Max(x1, x2);

    [DSLFunction]
    public nuint Max(nuint x1, nuint x2) => System.Math.Max(x1, x2);

    #endregion // Max

    #region Min

    [DSLFunction]
    public int Min(int x1, int x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public float Min(float x1, float x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public short Min(short x1, short x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public long Min(long x1, long x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public nint Min(nint x1, nint x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public sbyte Min(sbyte x1, sbyte x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public decimal Min(decimal x1, decimal x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public double Min(double x1, double x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public byte Min(byte x1, byte x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public ushort Min(ushort x1, ushort x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public uint Min(uint x1, uint x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public ulong Min(ulong x1, ulong x2) => System.Math.Min(x1, x2);

    [DSLFunction]
    public nuint Min(nuint x1, nuint x2) => System.Math.Min(x1, x2);
    #endregion // Min

    #region Clamp

    [DSLFunction]
    public byte Clamp(byte x, byte min, byte max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public decimal Clamp(decimal x, decimal min, decimal max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public double Clamp(double x, double min, double max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public short Clamp(short x, short min, short max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public int Clamp(int x, int min, int max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public long Clamp(long x, long min, long max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public nint Clamp(nint x, nint min, nint max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public sbyte Clamp(sbyte x, sbyte min, sbyte max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public float Clamp(float x, float min, float max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public ushort Clamp(ushort x, ushort min, ushort max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public uint Clamp(uint x, uint min, uint max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public ulong Clamp(ulong x, ulong min, ulong max) => System.Math.Clamp(x, min, max);

    [DSLFunction]
    public nuint Clamp(nuint x, nuint min, nuint max) => System.Math.Clamp(x, min, max);

    #endregion // Clamp

    [DSLFunction]
    public decimal Floor(decimal x) => System.Math.Floor(x);

    #region Round

    [DSLFunction]
    public decimal Round(decimal x) => System.Math.Round(x);

    [DSLFunction]
    public decimal Round(decimal x, int decimals) => System.Math.Round(x, decimals);

    [DSLFunction]
    public double Round(double x) => System.Math.Round(x);

    [DSLFunction]
    public double Round(double x, int digits) => System.Math.Round(x, digits);

    #endregion // Round

    #region Sign

    [DSLFunction]
    public int Sign(decimal x) => System.Math.Sign(x);

    [DSLFunction]
    public int Sign(double x) => System.Math.Sign(x);

    [DSLFunction]
    public int Sign(short x) => System.Math.Sign(x);

    [DSLFunction]
    public int Sign(int x) => System.Math.Sign(x);

    [DSLFunction]
    public int Sign(long x) => System.Math.Sign(x);

    [DSLFunction]
    public int Sign(nint x) => System.Math.Sign(x);

    [DSLFunction]
    public int Sign(sbyte x) => System.Math.Sign(x);

    [DSLFunction]
    public int Sign(float x) => System.Math.Sign(x);

    #endregion // Sign

    #region Truncate

    [DSLFunction]
    public decimal Truncate(decimal x) => System.Math.Truncate(x);

    [DSLFunction]
    public double Truncate(double x) => System.Math.Truncate(x);

    #endregion // Truncate

    [DSLFunction]
    public double Cos(double x) => System.Math.Cos(x);

    [DSLFunction]
    public double Sin(double x) => System.Math.Sin(x);

    [DSLFunction]
    public double Tan(double x) => System.Math.Tan(x);

    [DSLFunction]
    public double Acos(double x) => System.Math.Acos(x);

    [DSLFunction]
    public double Asin(double x) => System.Math.Asin(x);

    [DSLFunction]
    public double Atan(double x) => System.Math.Atan(x);

    [DSLFunction]
    public double Atan2(double x1, double x2) => System.Math.Atan2(x2, x1);

    [DSLFunction]
    public double Cosh(double x) => System.Math.Cosh(x);

    [DSLFunction]
    public double Sinh(double x) => System.Math.Sinh(x);

    [DSLFunction]
    public double Tanh(double x) => System.Math.Tanh(x);

    [DSLFunction]
    public double Exp(double x) => System.Math.Exp(x);

    [DSLFunction]
    public double Pow(double x1, double x2) => System.Math.Pow(x1, x2);

    [DSLFunction]
    public double Sqrt(double x) => System.Math.Sqrt(x);
}
