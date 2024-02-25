using Emgu.CV;
using System.Drawing;

namespace QMTGroup.ImageFilters.Filters
{
    /// <summary>
    /// A simple GaussConvolution (3x3)
    /// </summary>
    public class ConvolutionGauss : Convolution
    {
        /*
        public ConvolutionGauss() : base(
            new ConvolutionKernelF(new float[3, 3]{
                    {1.0f/9.0f, 2.0f/9.0f, 1.0f/9.0f},
                    {2.0f/9.0f, 4.0f/9.0f, 2.0f/9.0f},
                    {1.0f/9.0f, 2.0f/9.0f, 1.0f/9.0f}},
                new Point(1, 1)))
        { }
        */

        public ConvolutionGauss() : base(
            new ConvolutionKernelF(new float[5, 5]{
                    {1.0f/273.0f, 4.0f/273.0f, 7.0f/273.0f, 4.0f/273.0f,1/273.0f},
                    {4.0f/273.0f,16.0f/273.0f,26.0f/273.0f,16.0f/273.0f,4/273.0f},
                    {7.0f/273.0f,26.0f/273.0f,41.0f/273.0f,26.0f/273.0f,7/273.0f},
                    {4.0f/273.0f,16.0f/273.0f,26.0f/273.0f,16.0f/273.0f,4/273.0f},
                    {1.0f/273.0f, 4.0f/273.0f, 7.0f/273.0f, 4.0f/273.0f,1/273.0f}},
                new Point(2, 2)))
        { }
    }
}
