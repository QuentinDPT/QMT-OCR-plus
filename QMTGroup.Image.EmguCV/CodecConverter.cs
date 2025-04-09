using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using QMTGroup.Image.Interface;

namespace QMTGroup.Image.EmguCV
{
    public class CodecConverter : IJpegConverter
    {
        private readonly int[] parameters = [(int)ImwriteFlags.JpegQuality, 95];

        public byte[] ConvertToJpeg(Matrix image, int quality)
        {
            try
            {
                parameters[1] = quality;
                Mat emguMatrix = image.ToMat().Clone();
                VectorOfByte jpegData = new VectorOfByte();
                CvInvoke.Imencode(".jpg", emguMatrix, jpegData);
                emguMatrix.Dispose();
                return jpegData.ToArray();
            }
            catch (Exception ex)
            {

            }
            return [];
        }

        public byte[] ConvertToJpeg(Matrix image) => ConvertToJpeg(image, 95);
    }
}
