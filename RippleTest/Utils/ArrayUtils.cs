namespace RippleTest.Classes;

public class ArrayUtils
{
    public static byte[] copyOfRange(byte[] src, int start, int end)
    {
        var len = end - start;
        var dest = new byte[len];
        Array.Copy(src, start, dest, 0, len);
        return dest;
    }
}