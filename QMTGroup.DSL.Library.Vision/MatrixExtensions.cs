using QMTGroup.Image;
using System.Drawing;
using System.Drawing.Imaging;

namespace QMTGroup.DSL.Library.Vision;

public static class MatrixExtensions
{
    public static string ToBase64(this Matrix self)
    {
        PixelFormat format;
        switch (self.Channels)
        {
            case 3:
                format = PixelFormat.Format24bppRgb;
                break;
            case 4:
                format = PixelFormat.Format32bppArgb;
                break;
            default:
                throw new ArgumentException("Unsupported number of channels.");
        }

        int width = self.Width;
        int height = self.Height;

        Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pixelRed = self.Data[(self.Width * y + x) * self.Channels];
                var pixelBlue = self.Data[(self.Width * y + x) * self.Channels +1];
                var pixelGreen = self.Data[(self.Width * y + x) * self.Channels +2];

                int argb = (pixelGreen << (8 * 2)) | (pixelBlue << (8)) | pixelRed;
                Color color = Color.FromArgb(argb);
                bitmap.SetPixel(x, y, color);
            }
        }

        using (MemoryStream ms = new MemoryStream())
        {
            // Choisis le format (par exemple PNG pour conserver la transparence)
            bitmap.Save(ms, ImageFormat.Png);
            byte[] imageBytes = ms.ToArray();

            // Convertir en Base64
            return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
        }
    }
}
