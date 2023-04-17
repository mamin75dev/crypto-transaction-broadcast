using NBitcoin;

namespace RippleTest.Dto;

public class TransactionCachedData
{
    public TransactionBuilder Rebuild { get; set; }
    public Transaction UnsignedTransaction { get; set; }
    public List<Coin> UnspentCoins { get; set; }
    public int Index { get; set; }
    public string PublicKey { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public decimal Value { get; set; }
    public string Asset { get; set; }
    public string Net { get; set; }
    public object NetType { get; set; }
    public string Hash { get; set; }
}