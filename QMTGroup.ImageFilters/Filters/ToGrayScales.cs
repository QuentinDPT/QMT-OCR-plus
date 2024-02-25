using Emgu.CV;
using Emgu.CV.Structure;
using QMTGroup.Models.ImageFilters;

namespace QMTGroup.ImageFilters.Filters
{
    public class ToGrayScales : IImageFilter
    {
        public Mat ApplyFilter(Mat input)
        {
            return input.ToImage<Bgr, Byte>().Convert<Gray, byte>().Mat;
        }
    }
}
