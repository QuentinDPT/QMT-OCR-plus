namespace QMTGroup.Image.Interface;

public interface IJpegConverter
{
    byte[] ConvertToJpeg(Matrix image);

    byte[] ConvertToJpeg(Matrix image, int quality);
}
