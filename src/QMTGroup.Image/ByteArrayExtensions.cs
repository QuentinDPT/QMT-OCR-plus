namespace QMTGroup.Image;

public static class ByteArrayExtensions
{
    public static IntPtr ToIntPtr(this byte[] self)
    {

        IntPtr ptr;
        unsafe
        {
            fixed (byte* pData = self)
            {
                ptr = (IntPtr)pData;
            }
        }
        return ptr;
    }
}
