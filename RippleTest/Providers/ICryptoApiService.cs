using NBitcoin;

namespace RippleTest.Providers;

public interface ICryptoApiService
{
    Task<List<Coin>> GetSpendingCoins(string address, decimal amount);

    Task<List<Coin>> GetUnspentCoins(string address, string pubKey);
}