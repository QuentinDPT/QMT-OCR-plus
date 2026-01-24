using QMTGroup.Image;

namespace QMTGroup.ImageFilters.Filters;

public class ToGrayScales : IImageFilter
{
    public Matrix ApplyFilter(Matrix input)
    {
        return input;
        //return input.ToImage<Bgr, Byte>().Convert<Gray, byte>().Mat;
    }
}
