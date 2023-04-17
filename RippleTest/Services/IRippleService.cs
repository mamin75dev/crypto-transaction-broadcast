using Ripple.Binary.Codec.Types;

namespace RippleTest;

public interface IRippleService
{
    StObject SpendTransaction();

    void SaveTransaction(StObject obj);
}