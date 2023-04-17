using System.Text;

namespace RippleTest.Utils;

public class SmartCardHelper
{
    public static string ParseJavacardSignatureAndRemoveLeadingZeros(string javaCardSignature)
    {
        var signatureDictionary = ParseSignature(javaCardSignature);
        var str = RemovedLeadingZeros(signatureDictionary);

        return str.ToString();
    }

    private static Dictionary<string, string> ParseSignature(string javaCardSignature)
    {
        var signatureDictionary = new Dictionary<string, string>();
        signatureDictionary.Add(Constants.ECDSA_PART_ONE, javaCardSignature.Substring(0, 2));
        signatureDictionary.Add(Constants.ECDSA_PART_TWO, javaCardSignature.Substring(2, 2));
        signatureDictionary.Add(Constants.ECDSA_PART_THREE, javaCardSignature.Substring(4, 2));
        var rLenStr = javaCardSignature.Substring(6, 2);
        var rLen = Convert.ToInt32(rLenStr, 16);
        signatureDictionary.Add(Constants.ECDSA_PART_FOUR, rLenStr);
        signatureDictionary.Add(Constants.ECDSA_PART_FIVE, javaCardSignature.Substring(8, 2 * rLen));
        signatureDictionary.Add(Constants.ECDSA_PART_SIX, javaCardSignature.Substring(8 + 2 * rLen, 2));
        var sLenStr = javaCardSignature.Substring(10 + 2 * rLen, 2);
        var sLen = Convert.ToInt32(sLenStr, 16);
        signatureDictionary.Add(Constants.ECDSA_PART_SEVEN, sLenStr);
        signatureDictionary.Add(Constants.ECDSA_PART_EIGHT, javaCardSignature.Substring(12 + 2 * rLen, 2 * sLen));

        return signatureDictionary;
    }

    private static StringBuilder RemovedLeadingZeros(Dictionary<string, string> signatureDictionary)
    {
        var rLeadingZeroNumbers = numberOfLeadingZeros(signatureDictionary["r"]);
        var sLeadingZeroNumbers = numberOfLeadingZeros(signatureDictionary["s"]);

        var str = new StringBuilder();
        str.Append(signatureDictionary[Constants.ECDSA_PART_ONE]);
        var signatureLen = Convert.ToInt32(signatureDictionary[Constants.ECDSA_PART_TWO], 16) -
                           (rLeadingZeroNumbers / 2 + sLeadingZeroNumbers / 2);
        str.Append(signatureLen.ToString("X"));
        str.Append(signatureDictionary[Constants.ECDSA_PART_THREE]);
        var rLen = Convert.ToInt32(signatureDictionary[Constants.ECDSA_PART_FOUR], 16) - rLeadingZeroNumbers / 2;
        str.Append(rLen.ToString("X"));
        str.Append(signatureDictionary[Constants.ECDSA_PART_FIVE].Substring(rLeadingZeroNumbers));
        str.Append(signatureDictionary[Constants.ECDSA_PART_SIX]);
        var sLen = Convert.ToInt32(signatureDictionary[Constants.ECDSA_PART_SEVEN], 16) - sLeadingZeroNumbers / 2;
        str.Append(sLen.ToString("X"));
        str.Append(signatureDictionary[Constants.ECDSA_PART_EIGHT].Substring(sLeadingZeroNumbers));
        return str;
    }

    private static int numberOfLeadingZeros(string input)
    {
        var count = 0;
        var i = 0;

        while (input.Substring(i, 2).Equals("00"))
        {
            count += 2;
            i += 2;
        }

        return count;
    }
}