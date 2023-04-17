using System.Security.Cryptography;
using System.Text;

namespace RippleTest.Utils;

public static class HashGenerator
{
    public static byte[] Sha256(byte[] rawData)
    {
        using (var sha256Hash = SHA256.Create())
        {
            return sha256Hash.ComputeHash(rawData);
        }
    }

    public static byte[] DoubleSha256(byte[] rawData)
    {
        return Sha256(Sha256(rawData));
    }

    public static string DoubleSha256(string rawData)
    {
        return Convert.ToHexString(DoubleSha256(Encoding.UTF8.GetBytes(rawData)));
    }
}