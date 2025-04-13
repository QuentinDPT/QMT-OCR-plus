using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
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
                return CvInvoke.Imencode(".jpg", emguMatrix , new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.JpegQuality, quality));
            }
            catch (Exception ex)
            {

            }
            return [];
        }

        public byte[] ConvertToJpeg(Matrix image) => ConvertToJpeg(image, 95);
    }
}
