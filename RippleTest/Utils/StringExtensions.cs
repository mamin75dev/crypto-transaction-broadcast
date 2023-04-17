using System.Text;

namespace RippleTest.Classes;

public static class StringExtensions
{
    public static string ToHexString(this byte[] bytes)
    {
        var stringBuilder = new StringBuilder(bytes.Length * 2);
        foreach (var num in bytes)
            stringBuilder.AppendFormat("{0:x2}", num);
        return stringBuilder.ToString();
    }

    public static byte[] FromHexToByteArray(this string input)
    {
        var length = input.Length;
        var byteArray = new byte[length / 2];
        for (var startIndex = 0; startIndex < length; startIndex += 2)
            byteArray[startIndex / 2] = Convert.ToByte(input.Substring(startIndex, 2), 16);
        return byteArray;
    }
}