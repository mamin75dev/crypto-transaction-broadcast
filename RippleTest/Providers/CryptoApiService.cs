using CryptoAPIs.Api;
using CryptoAPIs.Client;
using NBitcoin;

namespace RippleTest.Providers;

public class CryptoApiService : ICryptoApiService

{
    // public List<Coin> GetUnspentCoins(string address, string pubKey)
    // {
    //     var uri = $"https://rest.cryptoapis.io/blockchain-data/dogecoin/testnet/addresses/{address}/unspent-outputs";
    //     var client = new RestClient(uri, configureSerialization: s => s.UseNewtonsoftJson());
    //
    //
    //     var req = new RestRequest { Method = Method.Get };
    //     req.AddHeader("X-API-Key", "875d804f43345bffca02e1500be3fb90e06bad6b");
    //
    //     var response = client.Execute(req);
    //
    //     var result = JsonConvert.DeserializeObject<DogecoinCryptoApiResponseDto>(response.Content);
    //
    //     // var jsonResponse = JObject.Parse(response.Content);
    //     //
    //     // var data = jsonResponse["data"]["items"];
    //     //
    //     // var unspentCoins = new List<Coin>();
    //     //
    //     // foreach (JObject trx in data)
    //     // {
    //     //     var unspentCoin = new Coin(
    // uint256.Parse((string)trx["transactionId"]),
    // uint.Parse((string)trx["index"]),
    // new Money(Convert.ToDecimal((string)trx["amount"]), MoneyUnit.BTC),
    // Script.FromHex(pubKey)
    //     //     );
    //     //     unspentCoins.Add(unspentCoin);
    //     // }
    //
    //     return null;
    //     // return unspentCoins;
    // }
    public async Task<List<Coin>> GetUnspentCoins(string address, string pubKey)
    {
        var config = new Configuration();
        config.BasePath = "https://rest.cryptoapis.io";
        config.AddApiKey("x-api-key", "875d804f43345bffca02e1500be3fb90e06bad6b");

        var apiInstance = new UnifiedEndpointsApi(config);
        var blockchian = "dogecoin";
        var network = "testnet";

        var unspentCoins = new List<Coin>();
        try
        {
            var result =
                await apiInstance.ListUnspentTransactionOutputsByAddressAsync(blockchian, network, address);

            var transactions = result.Data.Items;
            foreach (var trx in transactions)
            {
                var coin = new Coin(
                    uint256.Parse(trx.TransactionId),
                    uint.Parse(trx.Index.ToString()),
                    new Money(Convert.ToDecimal(trx.Amount), MoneyUnit.BTC),
                    Script.FromHex(pubKey)
                );
                unspentCoins.Add(coin);
            }
        }
        catch (ApiException e)
        {
            Console.WriteLine(e);
            throw;
        }

        return unspentCoins;
    }
}