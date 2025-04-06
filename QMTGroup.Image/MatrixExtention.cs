namespace QMTGroup.Image;


public static class MatrixExtention
{
    public static byte[] GetPixel(this Matrix self, uint x, uint y)
    {
        if (x > self.Width)
            throw new ArgumentOutOfRangeException(nameof(x), "x must be less than Width");

        if (y > self.Height)
            throw new ArgumentOutOfRangeException(nameof(y), "y must be less than Height");

        throw new NotImplementedException();
        /*
        return self.Data
            .Skip((int)(self.Width * y + x) * (int)self.Channels)
            .Take((int)self.Channels)
            .ToArray();
        //*/
    }

    public static void SetPixel(this Matrix self, uint x, uint y, byte[] pixel)
    {
        if (x > self.Width)
            throw new ArgumentOutOfRangeException(nameof(x), "x must be less than Width");

        if (y > self.Height)
            throw new ArgumentOutOfRangeException(nameof(y), "y must be less than Height");

        if (pixel.Length != self.Channels)
            throw new ArgumentOutOfRangeException(nameof(pixel.Length), "the pixel size must match the image channel size");

        for (int i = 0; i < pixel.Length; i++)
        {
            self.Data[(int)(self.Width * y + x) * (int)self.Channels + i] = pixel[i];
        }
    }

}

