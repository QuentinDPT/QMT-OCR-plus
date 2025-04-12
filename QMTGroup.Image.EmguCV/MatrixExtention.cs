using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QMTGroup.Image;

public static class MatrixExtention
{
    public static Matrix FromMat(Mat mat)
    {
        DataType dataType;
        switch (mat.NumberOfChannels)
        {
            case 1:
                dataType = DataType.Y_8;
                break;
            case 3:
                dataType = DataType.BGR_8;
                break;
            case 4:
                dataType = DataType.XRGB_8;
                break;
            default:
                throw new NotImplementedException();
        }

        var matrix = new Matrix(mat.Width, mat.Height, dataType);

        matrix.SetData(mat.GetRawData());

        return matrix;
    }

    public static Matrix ToMatrix(this Mat self)
    {
        return FromMat(self);
    }

    public static Mat ToMat(this Matrix self)
    {
        return new Mat(new Size(self.Width, self.Height), DepthType.Cv8U, self.Channels, self.ToIntPtr(), self.Width * self.Channels);
    }
}

