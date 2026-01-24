using QMTGroup.Core;
using System.Runtime.InteropServices;

namespace QMTGroup.Image2;

public class Matrix : SafeHandle, ICloneable<Matrix>
{
    /// <summary>
    /// The first memory cell of the image stored.
    /// </summary>
    public IntPtr Handle => _handle;

    private IntPtr _handle;

    internal protected Matrix(IntPtr handle)
        : base(handle, true)
        => _handle = handle;

    /// <summary>
    /// Instanciates a matrix on unmanaged memory.
    /// </summary>
    /// <param name="width">Width of the matrix.</param>
    /// <param name="height">Height of the matrix.</param>
    /// <param name="channelType">The <see cref="ChannelType">ChannelType</see> that organize matrix memory cells.</param>
    /// <exception cref="ArgumentOutOfRangeException">If any of the <paramref name="width"/> or <paramref name="height"/> is negative.</exception>
    /// <exception cref="Exception">When something went wrong during the native call.</exception>
    public Matrix(int width, int height, ChannelType channelType)
        : base(IntPtr.Zero, true)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width, nameof(width));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height, nameof(height));

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(width, int.MaxValue);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(height, int.MaxValue);
        long totalPixels = (long)width * (long)height;
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(totalPixels, int.MaxValue);

        _handle = NativeMatrix.matrix_new((UIntPtr)width, (UIntPtr)height, (byte)channelType);
        SetHandle(_handle);

        if (_handle == IntPtr.Zero)
            throw new Exception("Matrix allocation failed");
    }

    /// <summary>
    /// Frees former matrix and set the new one to a raw unmanaged memory.
    /// </summary>
    /// <param name="handle">New unmanaged memory location of the new matrix.</param>
    /// <exception cref="Exception">When something went wrong during the native call.</exception>
#warning Potentially deprecated
// var img = new Matrix();
// var img2 = img;
// img2.ReAlloc(IntPtr.Any);
// > img is free here
    protected void ReAlloc(IntPtr handle)
    {
        if (_handle != IntPtr.Zero)
        {
            NativeMatrix.matrix_free(_handle);
        }
        _handle = handle;
        if (_handle == IntPtr.Zero)
            throw new Exception("Matrix reallocation failed");
    }

    public void SetData(byte[] data)
    {
        int result = NativeMatrix.matrix_set_data(_handle, data, (UIntPtr)data.Length);

        if (result != 0)
            throw new Exception("Matrix data size mismatch");
    }

    public int Width => (int)NativeMatrix.matrix_get_width(_handle);

    public int Height => (int)NativeMatrix.matrix_get_height(_handle);

    public ChannelType ChannelType => (ChannelType)NativeMatrix.matrix_get_channel_type(_handle);

    public int ChannelSize => NativeMatrix.matrix_get_channel_size(_handle);

    public override bool IsInvalid => _handle == IntPtr.Zero;

    public Matrix Clone()
        => new Matrix(NativeMatrix.matrix_clone(_handle));

    protected override bool ReleaseHandle()
    {
        if (_handle != IntPtr.Zero)
        {
            NativeMatrix.matrix_free(_handle);
            _handle = IntPtr.Zero;
        }
        return true;
    }
}

public enum ChannelType : byte
{
    Y8,
    Rgb8,
    Bgr8,
    Xrgb8,
    Rgbx8,
    Rgba8,
    Xbgr8,
    Bgrx8,
    Bgra8,
}

internal static class NativeMatrix
{
    const string DllName = "qmtgroup_matrix_rs.dll";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr matrix_new(UIntPtr width, UIntPtr height, byte channelType);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr matrix_clone(IntPtr matrix);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int matrix_set_data(IntPtr matrix, byte[] data, UIntPtr len);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr matrix_get_width(IntPtr matrix);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr matrix_get_height(IntPtr matrix);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern byte matrix_get_channel_type(IntPtr matrix);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern byte matrix_get_channel_size(IntPtr matrix);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void matrix_free(IntPtr matrix);
}
