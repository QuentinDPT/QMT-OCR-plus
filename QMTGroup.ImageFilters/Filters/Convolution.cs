using Emgu.CV;
using Emgu.CV.Structure;
using QMTGroup.Models.ImageFilters;
using System.Drawing;

namespace QMTGroup.ImageFilters.Filters
{
    public class Convolution : IImageFilter
    {
        private ConvolutionKernelF _kernel;

        /// <summary>
        /// Get or set the convolution kernel
        /// </summary>
        protected ConvolutionKernelF Kernel
        {
            get => _kernel;
            set => _kernel = value;
        }


        /// <summary>
        /// Apply a convolution filter on your image
        /// </summary>
        /// <param name="kernel">The convolution matrice</param>
        public Convolution(ConvolutionKernelF kernel)
        {
            if(kernel == null)
                throw new ArgumentNullException("kernel");
            if (kernel.Width % 2 != 1 || kernel.Height % 2 != 1)
                throw new ArgumentException("kernel should have impair rows and columns");

            _kernel = kernel;
        }

        public Mat ApplyFilter(Mat input)
        {
            return input.ToImage<Bgr, Byte>().Convolution(_kernel).Mat;
        }
    }
}
