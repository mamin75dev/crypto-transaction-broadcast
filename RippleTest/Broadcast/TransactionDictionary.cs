using System.Collections.Concurrent;
using NBitcoin;
using RippleTest.Dto;

namespace RippleTest.Broadcast;

public class TransactionDictionary
{
    private static ConcurrentDictionary<string, TransactionCachedData> TransactionData;

    public TransactionDictionary()
    {
        if (TransactionData == null) TransactionData = new ConcurrentDictionary<string, TransactionCachedData>();
    }

    public void AddToDictionary(string key, TransactionCachedData cachedData)
    {
        TransactionData.TryAdd(key, cachedData);
    }

    public void RemoveFromDictionary(string key)
    {
        TransactionData.TryRemove(key, out _);
    }

    public bool IsNull()
    {
        return TransactionData == null;
    }

    public TransactionCachedData GetTransactionCachedData(string key)
    {
        return TransactionData.TryGet(key);
    }

    public void AddTransactionCachedData(string publicKey, string hash, List<Coin> unspentCoins,
        Transaction unsignedTransaction, TransactionBuilder rebuild, int index, string from, string to, decimal value,
        string asset, string net)
    {
        var cachedData = new TransactionCachedData
        {
            Rebuild = rebuild,
            UnsignedTransaction = unsignedTransaction,
            UnspentCoins = unspentCoins,
            Index = index,
            PublicKey = publicKey,
            From = from,
            To = to,
            Value = value,
            Asset = asset,
            Net = net,
            NetType = "testnet",
            Hash = hash
        };

        AddToDictionary($"{hash}", cachedData);
    }
}