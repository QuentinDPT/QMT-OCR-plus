namespace QMTGroup.Image.EmguCV.Test
{
    public class CodecConverterTest
    {
        private readonly Matrix matrix;

        public CodecConverterTest()
        {
            matrix = new Matrix(500,500);
        }

        [Fact]
        public void OnConvertToJpeg_WhenInputIsConsistent_ShouldReturnJpeg()
        {
            var codecConverter = new CodecConverter();

            codecConverter.ConvertToJpeg(matrix);
        }

        [Theory]
        [InlineData(1000000)]
        public void OnConvertToJpeg_WhenInputIsConsistentWithLoad_ShouldReturnJpeg(int jpegLoad)
        {
            var codecConverter = new CodecConverter();

            for (int i = 0; i < jpegLoad; i++)
            {
                codecConverter.ConvertToJpeg(matrix);
            }
        }
    }
}