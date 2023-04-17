using Ripple.Binary.Codec.Types;

namespace RippleTest;

public class RippleService : IRippleService
{
    private StObject obj { get; set; }

    public StObject SpendTransaction()
    {
        return obj;
    }

    public void SaveTransaction(StObject stObject)
    {
        obj = stObject;
    }
}