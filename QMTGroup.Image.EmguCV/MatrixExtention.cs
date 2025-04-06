using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace QMTGroup.Image;

public static class MatrixExtention
{
    public static Matrix FromMat(Mat mat)
    {
        Type ChannelType;
        switch (mat.Depth)
        {
            case DepthType.Cv8U:
                ChannelType = typeof(byte);
                break;
            case DepthType.Cv8S:
                ChannelType = typeof(sbyte);
                break;
            case DepthType.Cv16U:
                ChannelType = typeof(ushort);
                break;
            case DepthType.Cv16S:
                ChannelType = typeof(short);
                break;
            case DepthType.Cv32S:
                ChannelType = typeof(int);
                break;
            case DepthType.Cv32F:
                ChannelType = typeof(float);
                break;
            case DepthType.Cv64F:
                ChannelType = typeof(double);
                break;
            default:
                throw new TypeLoadException("Unsupported type.");
        }

        var matrix = new Matrix(mat.Width, mat.Height, mat.NumberOfChannels)
        {
            ChannelType = ChannelType,
        };
        matrix.SetData(mat.GetRawData());

        return matrix;
    }

    public static Matrix ToMatrix(this Mat self)
    {
        return FromMat(self);
    }

    public static Mat ToMat(this Matrix self)
    {
        DepthType depthType;

        if (self.ChannelType == typeof(byte))
        {
            depthType = DepthType.Cv8U;
        }
        else if (self.ChannelType == typeof(sbyte))
        {
            depthType = DepthType.Cv8S;
        }
        else if (self.ChannelType == typeof(ushort))
        {
            depthType = DepthType.Cv16U;
        }
        else if (self.ChannelType == typeof(short))
        {
            depthType = DepthType.Cv16S;
        }
        else if (self.ChannelType == typeof(int))
        {
            depthType = DepthType.Cv32S;
        }
        else if (self.ChannelType == typeof(float))
        {
            depthType = DepthType.Cv32F;
        }
        else if (self.ChannelType == typeof(double))
        {
            depthType = DepthType.Cv64F;
        }
        else
        {
            throw new TypeLoadException("Unsupported type.");
        }

        //return new Mat([(int)self.Height, (int)self.Width, (int)self.Channels], depthType, self.GetDataPtr());

        return new Mat(new Size((int)self.Height, (int)self.Width), depthType, (int)self.Channels, self.GetDataPtr(), (int)self.Width * (int)self.Channels);

        return new Mat(new Size((int)self.Height, (int)self.Width), depthType, (int)self.Channels, self.GetDataPtr(), 1);
    }

    public static Mat ToMatCopy(this Matrix self)
    {
        DepthType depthType;

        if (self.ChannelType == typeof(byte))
        {
            depthType = DepthType.Cv8U;
        }
        else if (self.ChannelType == typeof(sbyte))
        {
            depthType = DepthType.Cv8S;
        }
        else if (self.ChannelType == typeof(ushort))
        {
            depthType = DepthType.Cv16U;
        }
        else if (self.ChannelType == typeof(short))
        {
            depthType = DepthType.Cv16S;
        }
        else if (self.ChannelType == typeof(int))
        {
            depthType = DepthType.Cv32S;
        }
        else if (self.ChannelType == typeof(float))
        {
            depthType = DepthType.Cv32F;
        }
        else if (self.ChannelType == typeof(double))
        {
            depthType = DepthType.Cv64F;
        }
        else
        {
            throw new TypeLoadException("Unsupported type.");
        }

        Mat result = new Mat((int)self.Height, (int)self.Width, depthType, (int)self.Channels);

        result.SetTo(self.Data.ToArray());

        return result;
    }
}

