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
        //return new Mat([(int)self.Height, (int)self.Width, (int)self.Channels], _getEmguDepthType(self.ChannelType), self.GetDataPtr());

        return new Mat(new Size((int)self.Height, (int)self.Width), _getEmguDepthType(self.ChannelType), (int)self.Channels, self.ToIntPtr(), (int)self.Width * (int)self.Channels);

        return new Mat(new Size((int)self.Height, (int)self.Width), _getEmguDepthType(self.ChannelType), (int)self.Channels, self.ToIntPtr(), 1);
    }

    public static Mat ToMatCopy(this Matrix self)
    {
        byte[] b = self.Data.ToArray();

        //return new Mat(new Size(self.Height, self.Width), _getEmguDepthType(self.ChannelType), self.Channels, self.GetDataPtr(), self.Width * self.Channels);

        return new Mat(new Size(self.Width, self.Height), DepthType.Cv8U, self.Channels, b.ToIntPtr(), self.Width * self.Channels);
    }

    private static DepthType _getEmguDepthType(DataType chanelType)
    {
        switch (chanelType)
        {
            case DataType.Y_8:
                return DepthType.Cv8U;
            case DataType.RGB_8:
            case DataType.BGR_8:
                return DepthType.Cv32F;
            case DataType.XRGB_8:
                return DepthType.Cv64F;
            default:
                throw new NotImplementedException();
        }
    }
}

