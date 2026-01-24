namespace Image.IPCPOC
{
    public record Image
    {
        IntPtr ByteArrayLocation;

        int ByteArraySize;

        /// <summary>
        /// RGBA, RGB, XRGB, G
        /// </summary>
        byte ByteArrayTemplate;

        int Width;

        int Height;
    }
}
