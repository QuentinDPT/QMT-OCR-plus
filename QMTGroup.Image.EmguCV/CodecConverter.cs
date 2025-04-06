using Emgu.CV.Structure;
using QMTGroup.Image.Interface;

namespace QMTGroup.Image.EmguCV
{
    public class CodecConverter : IJpegConverter
    {
        public byte[] ConvertToJpeg(Matrix image, int quality)
        {
            if(image.Channels == 1)
                return image.ToMat().ToImage<Gray, byte>().ToJpegData(quality);

            if (image.Channels == 3)
                return image.ToMat().ToImage<Bgr, byte>().ToJpegData(quality);

            return image.ToMat().ToImage<Gray, byte>().ToJpegData(quality);
        }
    }
}
