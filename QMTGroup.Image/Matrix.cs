namespace QMTGroup.Image;

public record Matrix
{
    /// <summary>
    /// Raw data of the image.
    /// </summary>
    public byte[] Data
    {
        get => _data;
        init => _data = value;
    }
    private byte[] _data;

    /// <summary>
    /// Number of channels in the image.<br/>
    /// 3 for RGB, 4 for RGBA, 1 for grayscale.
    /// </summary>
    public uint Channels
    {
        get => _channels;
        init => _channels = value;
    }
    private uint _channels;

    /// <summary>
    /// Type of all channels.
    /// </summary>
    public Type ChannelType
    {
        get => _channelType;
        init => _channelType = value;
    }
    private Type _channelType;

    /// <summary>
    /// Width of the image in pixels.
    /// </summary>
    public uint Width
    {
        get => _width;
        init => _width = value;
    }
    private uint _width;

    /// <summary>
    /// Height of the image in pixels.
    /// </summary>
    public uint Height
    {
        get => _height;
        init => _height = value;
    }
    private uint _height;
}
