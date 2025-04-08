using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using QMTGroup.Image.Interface;

namespace QMTGroup.Image.EmguCV
{
    public class CodecConverter : IJpegConverter
    {
        private readonly int[] parameters = [(int)ImwriteFlags.JpegQuality, 95];
        private readonly Mat _sharedMat = new Mat();

        public byte[] ConvertToJpeg(Matrix image, int quality)
        {
            try
            {
                parameters[1] = quality;
                Mat emguMatrix = image.ToMatCopy();
                VectorOfByte jpegData = new VectorOfByte();
                CvInvoke.Imencode(".jpg", emguMatrix, jpegData);
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
