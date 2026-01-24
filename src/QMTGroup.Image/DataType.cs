namespace QMTGroup.Image;

/// <summary>
/// Represents the data organisation behing the data array<br/>
/// <c>R</c> is a red channel,<br/>
/// <c>G</c> is a green channel,<br/>
/// <c>B</c> is a blue channel,<br/>
/// <c>A</c> is an opacity channel,<br/>
/// <c>X</c> is an unused channel,<br/>
/// <c>Y</c> is a gray channel<br/>
/// </summary>
public enum DataType
{
    /// <summary>
    /// Gray, total 8 bit<br/>
    /// <c>Y[8]</c>
    /// </summary>
    Y_8,

    /// <summary>
    /// RGB, total 24 bit<br/>
    /// <c>R[8], G[8], B[8]</c>
    /// </summary>
    RGB_8,

    /// <summary>
    /// BGR, total 24 bit<br/>
    /// <c>B[8], G[8], R[8]</c>
    /// </summary>
    BGR_8,

    /// <summary>
    /// RGB, total 32 bit<br/>
    /// <c>X[8], R[8], G[8], B[8]</c>
    /// </summary>
    XRGB_8,

    /// <summary>
    /// RGB, total 32 bit<br/>
    /// <c>R[8], G[8], B[8], X[8]</c>
    /// </summary>
    RGBX_8,

    /// <summary>
    /// RGBA, total 32 bit<br/>
    /// <c>R[8], G[8], B[8], A[8]</c>
    /// </summary>
    RGBA_8,


    /// <summary>
    /// BGR, total 32 bit<br/>
    /// <c>X[8], B[8], G[8], R[8]</c>
    /// </summary>
    XBGR_8,

    /// <summary>
    /// BGR, total 32 bit<br/>
    /// <c>B[8], G[8], R[8], X[8]</c>
    /// </summary>
    BGRX_8,

    /// <summary>
    /// BGRA, total 32bit<br/>
    /// <c>B[8], R[8], G[8], A[8]</c>
    /// </summary>
    BGRA_8,
}
