using System;
using System.Buffers;
using System.Drawing;
using System.Runtime.InteropServices;

namespace QMTGroup.Image;

public record Matrix : IDisposable
{
    private ArrayPool<byte> _memoryPool = ArrayPool<byte>.Shared;

    public Matrix()
    {
        _dataLength = 1;
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public Matrix(int imageSize)
    {
        _dataLength = imageSize;
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public Matrix(int width, int height, int channels = 1)
    {
        Width = (uint)width;
        Height = (uint)height;
        Channels = (uint)channels;

        _dataLength = width * height * channels;

        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public Matrix(ArrayPool<byte> memoryPool)
    {
        _memoryPool = memoryPool;
        _dataLength = 1;
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public Matrix(ArrayPool<byte> memoryPool, int imageSize)
    {
        _memoryPool = memoryPool;
        _dataLength = imageSize;
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public Matrix(ArrayPool<byte> memoryPool, int width, int height, int channels = 1)
    {
        _memoryPool = memoryPool;
        _dataLength = width * height * channels;
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    /// <summary>
    /// Raw data of the image.
    /// </summary>
    public Span<byte> Data
    {
        get => new Span<byte>(_innerDataSpace, 0, _dataLength);
    }
    private byte[] _innerDataSpace;
    private int _dataLength = 0;

    /// <summary>
    /// Number of channels in the image.<br/>
    /// 3 for RGB, 4 for RGBA, 1 for grayscale.
    /// </summary>
    public uint Channels
    {
        get => _channels;
        set => _channels = value;
    }
    private uint _channels = 1;

    /// <summary>
    /// Type of all channels.
    /// </summary>
    public Type ChannelType
    {
        get => _channelType;
        set => _channelType = value;
    }
    private Type _channelType;

    /// <summary>
    /// Width of the image in pixels.
    /// </summary>
    public uint Width
    {
        get => _width;
        set => _width = value;
    }
    private uint _width;

    /// <summary>
    /// Height of the image in pixels.
    /// </summary>
    public uint Height
    {
        get => _height;
        set => _height = value;
    }
    private uint _height;

    public void SetDataSize(int size)
    {
        _dataLength = size;
        _memoryPool.Return(_innerDataSpace);
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public void SetData(byte[] data) => Buffer.BlockCopy(data, 0, _innerDataSpace, 0, _dataLength);

    public void SetData(nint data) => Marshal.Copy(data, _innerDataSpace, 0, _dataLength);

    public void SetData(Span<byte> data) => data.CopyTo(_innerDataSpace);

    public IntPtr GetDataPtr()
    {
        IntPtr ptr;
        unsafe
        {
            fixed (byte* pData = Data)
            {
                ptr = (IntPtr)pData;
            }
        }
        return ptr;
    }

    public void Dispose()
    {
        _memoryPool.Return(_innerDataSpace);
    }
}
