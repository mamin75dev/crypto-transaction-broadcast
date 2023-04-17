using Org.BouncyCastle.Math;

namespace RippleTest.Classes;

public class SignatureUtils
{
    private static readonly string maxValueForS = "7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5D576E7357A4501DDFE92F46681B20A0";
    private static readonly string sThreshold = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141";

    public static BigInteger[] ExtractRSFromSignature(string signature)
    {
        var pos = 0;
        byte[] s;
        var signBytes = signature.FromHexToByteArray();
        int header = signBytes[pos++];
        if (header != 0x30) throw new Exception("Wrong signature format");
        int totalLength = signBytes[pos++];

        if (ArrayUtils.copyOfRange(signBytes, pos, signBytes.Length).Length != totalLength)
            throw new Exception("Wrong signature length");

        pos++;

        int rLength = signBytes[pos++];
        var r = ArrayUtils.copyOfRange(signBytes, pos, pos + rLength);
        pos += rLength;

        pos++;
        int sLength = signBytes[pos++];
        s = ArrayUtils.copyOfRange(signBytes, pos, pos + sLength);

        var bigR = new BigInteger(r);
        BigInteger bigS;

        if (s[0] == 0x00)
        {
            var newS = ArrayUtils.copyOfRange(s, 1, sLength);
            bigS = new BigInteger(s);
            if (checkHighS(bigS)) bigS = new BigInteger(sThreshold.FromHexToByteArray()).Subtract(new BigInteger(newS));
        }
        else
        {
            bigS = new BigInteger(s);
        }

        return new[] { bigR, bigS };
    }

    private static bool checkHighS(BigInteger bigS)
    {
        return bigS.CompareTo(new BigInteger(maxValueForS.FromHexToByteArray())) > 0;
    }
}