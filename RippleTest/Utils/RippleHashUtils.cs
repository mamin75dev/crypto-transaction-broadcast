using Ripple.Binary.Codec.Hashing;
using Ripple.Binary.Codec.Util;

namespace RippleTest.Utils;

public class RippleHashUtils
{
    public static string TransactionId(byte[] txBlob)
    {
        return B16.Encode(TransactionIdBytes(txBlob));
    }

    private static byte[] TransactionIdBytes(byte[] txBlob)
    {
        return Sha512.Half(txBlob, 1415073280U);
    }
}