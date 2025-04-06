using Emgu.CV;
using Emgu.CV.Structure;
using QMTGroup.Image.Interface;

namespace QMTGroup.Image.EmguCV
{
    public class CodecConverter : IJpegConverter
    {
        public byte[] ConvertToJpeg(Matrix image, int quality)
        {
            try
            {
                Mat emguMatrix = image.ToMat();

                if (image.Channels == 1)
                    return emguMatrix.ToImage<Gray, byte>().ToJpegData(quality);

                if (image.Channels == 3)
                    return emguMatrix.ToImage<Bgr, byte>().ToJpegData(quality);

                return emguMatrix.ToImage<Gray, byte>().ToJpegData(quality);
            }
            catch (Exception ex)
            {

            }
            return [];
        }

        public byte[] ConvertToJpeg(Matrix image) => ConvertToJpeg(image, 95);
    }
}
