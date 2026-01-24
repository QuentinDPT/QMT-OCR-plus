namespace QMTGroup.Image;

public static class DataTypeExtensions
{
    public static int GetPixelBitSize(this DataType self)
    {
        switch (self)
        {
            case DataType.Y_8:
                return 8;
            case DataType.RGB_8:
            case DataType.BGR_8:
                return 24;
            case DataType.XRGB_8:
            case DataType.RGBX_8:
            case DataType.RGBA_8:
            case DataType.BGRX_8:
            case DataType.BGRA_8:
                return 32;
            default:
                throw new NotImplementedException();
        }
    }
    public static int GetPixelByteSize(this DataType self)
    {
        return self.GetPixelBitSize() / 8;
    }
}
