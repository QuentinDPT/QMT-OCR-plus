using System.Buffers;

namespace QMTGroup.Image;

public record Matrix : IDisposable
{
    private ArrayPool<byte> _memoryPool = ArrayPool<byte>.Shared;

    public Matrix()
    {
        _data = _memoryPool.Rent(1);
    }

    public Matrix(int imageSize)
    {
        _data = _memoryPool.Rent(imageSize);
    }

    public Matrix(int width, int height, int channels = 1)
    {
        Width = (uint)width;
        Height = (uint)height;
        Channels = (uint)channels;

        _data = _memoryPool.Rent(width * height * channels);
    }

    public Matrix(ArrayPool<byte> memoryPool)
    {
        _memoryPool = memoryPool;
        _data = _memoryPool.Rent(1);
    }

    public Matrix(ArrayPool<byte> memoryPool, int imageSize)
    {
        _memoryPool = memoryPool;
        _data = _memoryPool.Rent(imageSize);
    }

    public Matrix(ArrayPool<byte> memoryPool, int width, int height, int channels = 1)
    {
        _memoryPool = memoryPool;
        _data = _memoryPool.Rent(width * height * channels);
    }

    /// <summary>
    /// Raw data of the image.
    /// </summary>
    public byte[] Data
    {
        get => _data;
        set => _data = value;
    }
    private byte[] _data;

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

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(_data);
    }
}
