using Emgu.CV;
using System.Drawing;

namespace QMTGroup.ImageFilters.Filters
{
    /// <summary>
    /// A simple GaussConvolution
    /// </summary>
    public class ConvolutionGauss : Convolution
    {

        /// <summary>
        /// GaussConvolution (5x5)
        /// </summary>
        public ConvolutionGauss() : base(
            new ConvolutionKernelF(
                _generateGaussianKernel(5, 1.5f),
                new Point(2, 2)))
        { }


        /// <summary>
        /// GaussConvolution
        /// </summary>
        /// <param name="size">Size of the convolution kernel</param>
        /// <param name="sigma">Ecart type</param>
        public ConvolutionGauss(int size, float sigma = 1.5f) : base(
            new ConvolutionKernelF(
                _generateGaussianKernel(size, sigma),
                new Point((int)Math.Truncate(size / 2f), (int)Math.Truncate(size / 2f))))
        { }

        private static float[,] _generateGaussianKernel(int size, float sigma)
        {
            float[,] kernel = new float[size, size];
            float sum = 0;
            int halfSize = size / 2;

            for (int i = -halfSize; i <= halfSize; i++)
            {
                for (int j = -halfSize; j <= halfSize; j++)
                {
                    float value = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)) / (2 * Math.PI * sigma * sigma));
                    kernel[i + halfSize, j + halfSize] = value;
                    sum += value;
                }
            }

            // Normalisation
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= sum;
                }
            }

            return kernel;
        }
    }
}
