using QMTGroup.Image.Interface;
using System.Buffers;
using System.Runtime.InteropServices;

namespace QMTGroup.Image;

public class Matrix : IDisposable, IClonable<Matrix>
{
    private ArrayPool<byte> _memoryPool = ArrayPool<byte>.Shared;

    public Matrix()
    {
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public Matrix(int imageSize)
    {
        _dataLength = imageSize;
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public Matrix(int width, int height, DataType channels = DataType.Y_8)
    {
        _width = width;
        _height = height;
        ChannelType = channels;

        _dataLength = _width * _height * _channels;

        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public Matrix(ArrayPool<byte> memoryPool)
    {
        _memoryPool = memoryPool;
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public Matrix(ArrayPool<byte> memoryPool, int imageSize)
    {
        _memoryPool = memoryPool;
        _dataLength = imageSize;
        _innerDataSpace = _memoryPool.Rent(_dataLength);
        
    }

    public Matrix(ArrayPool<byte> memoryPool, int width, int height, DataType channels = DataType.Y_8)
    {
        _memoryPool = memoryPool;
        _width = width;
        _height = height;
        ChannelType = channels;
        _dataLength = _width * _height * _channels;
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
    private int _dataLength = 1;

    /// <summary>
    /// Number of bytes for a pixel.
    /// </summary>
    public int Channels
    {
        get => _channels;
    }
    private int _channels = 1;

    /// <summary>
    /// Type of all channels.
    /// </summary>
    public DataType ChannelType
    {
        get => _channelType;
        private set
        {
            _channels = value.GetPixelByteSize();
            _channelType = value;
        }
    }
    private DataType _channelType = DataType.Y_8;

    /// <summary>
    /// Width of the image in pixels.
    /// </summary>
    public int Width
    {
        get => _width;
    }
    private int _width = 1;

    /// <summary>
    /// Height of the image in pixels.
    /// </summary>
    public int Height
    {
        get => _height;
    }
    private int _height = 1;

    public void SetDataSize(int size, int width, int height, DataType? dataType = null)
    {
        _width = width;
        _height = height;
        if (dataType != null)
            ChannelType = (DataType)dataType;
        _dataLength = size;
        _memoryPool.Return(_innerDataSpace);
        _innerDataSpace = _memoryPool.Rent(_dataLength);
    }

    public void SetData(byte[] data) => Buffer.BlockCopy(data, 0, _innerDataSpace, 0, _dataLength);

    public void SetData(nint data) => Marshal.Copy(data, _innerDataSpace, 0, _dataLength);

    public void SetData(Span<byte> data) => data.CopyTo(_innerDataSpace);

    public IntPtr ToIntPtr()
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

    public Matrix Clone()
    {
        var result = new Matrix(_memoryPool, _width, _height, _channelType);

        Data.CopyTo(result.Data);

        return result;
    }
}
