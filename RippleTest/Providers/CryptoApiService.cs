using CryptoAPIs.Api;
using CryptoAPIs.Client;
using CryptoAPIs.Model;
using NBitcoin;
using Newtonsoft.Json;
using RestSharp;
using RippleTest.Providers.Dto;

namespace RippleTest.Providers;

public class CryptoApiService : ICryptoApiService

{
    private UnifiedEndpointsApi apiInstance;

    private readonly IHttpClientFactory _httpClientFactory;
    private string blockchain = "dogecoin";
    private string network = "testnet";
    public CryptoApiService(IHttpClientFactory httpClientFactory)
    {
        var config = new Configuration();
        _httpClientFactory = httpClientFactory;
        config.BasePath = "https://rest.cryptoapis.io";
        // config.AddApiKey("x-api-key", "875d804f43345bffca02e1500be3fb90e06bad6b");
        config.AddApiKey("x-api-key", "e176369c3ee69c21ceebd77d43a04d07029e4c34");

        apiInstance = new UnifiedEndpointsApi(config);
    }

    public async Task<List<Coin>> GetUnspentCoins(string address, string pubKey)
    {
        /*var uri = $"https://rest.cryptoapis.io/blockchain-data/dogecoin/testnet/addresses/{address}/unspent-outputs";
        var client = new RestClient(uri, configureSerialization: s => s.UseNewtonsoftJson());


        var req = new RestRequest { Method = Method.Get };
        req.AddHeader("X-API-Key", "875d804f43345bffca02e1500be3fb90e06bad6b");

        var response = client.Execute(req);

        var result = JsonConvert.DeserializeObject<DogecoinCryptoApiResponseDto>(response.Content);*/

        // var jsonResponse = JObject.Parse(response.Content);
        //
        // var data = jsonResponse["data"]["items"];
        //
        // var unspentCoins = new List<Coin>();
        //
        // foreach (JObject trx in data)
        // {
        //     var unspentCoin = new Coin(
        // uint256.Parse((string)trx["transactionId"]),
        // uint.Parse((string)trx["index"]),
        // new Money(Convert.ToDecimal((string)trx["amount"]), MoneyUnit.BTC),
        // Script.FromHex(pubKey)
        //     );
        //     unspentCoins.Add(unspentCoin);
        // }


        var httpClinet = _httpClientFactory.CreateClient();

        httpClinet.DefaultRequestHeaders.Add("X-API-Key", "875d804f43345bffca02e1500be3fb90e06bad6b");

        var response = await httpClinet.GetAsync($"https://rest.cryptoapis.io/blockchain-data/dogecoin/testnet/addresses/{address}/unspent-outputs");


        var content = response.Content.ReadAsStringAsync();

        var result = content.Result;

        var transactions = JsonConvert.DeserializeObject<DogecoinCryptoApiResponseDto>(result);

        return null;
        // return unspentCoins;
    }

    public async Task<List<Coin>> GetSpendingCoins(string address, decimal amount)
    {

        var spendginCoins = new List<Coin>();
        try
        {
            var result =
                await apiInstance.ListUnspentTransactionOutputsByAddressAsync(blockchain, network, address);

            var transactions = result.Data.Items;

            var spendings = GetSpendingTransactions(transactions, amount);

            foreach (var trx in spendings)
            {

                var trxDetails = await GetTransactionDetailsByHash(trx.TransactionId);

                var output = trxDetails.Data.Item.BlockchainSpecific.Vout.Find(a => a.ScriptPubKey.Addresses.Contains(address));

                var script = output.ScriptPubKey.HEX;

                var coin = new Coin(
                    fromTxHash: uint256.Parse(trx.TransactionId),
                    fromOutputIndex: uint.Parse(trx.Index.ToString()),
                    amount: new Money(Convert.ToDecimal(trx.Amount), MoneyUnit.BTC),
                    scriptPubKey: Script.FromHex(script)
                );
                spendginCoins.Add(coin);
            }
        }
        catch (ApiException e)
        {
            Console.WriteLine(e);
            throw;
        }

        return spendginCoins;
    }

    // protected async Task<GetTransactionDetailsByTransactionIDR> GetTransactionDetailsByHash(string trxID)
    protected async Task<DogeTransactionDetails> GetTransactionDetailsByHash(string trxID)
    {
        /*try
        {
            var trxDetails = await apiInstance.GetTransactionDetailsByTransactionIDAsync(blockchain, network, trxID);
            var script = trxDetails.Data.Item.BlockchainSpecific;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }*/


        var url = $"https://rest.cryptoapis.io/blockchain-data/dogecoin/testnet/transactions/{trxID}";
        var client = new RestClient(url);


        var req = new RestRequest(Method.GET);
        req.AddHeader("X-API-Key", "e176369c3ee69c21ceebd77d43a04d07029e4c34");

        var response = client.Execute(req);

        var result = JsonConvert.DeserializeObject<DogeTransactionDetails>(response.Content);

        return result;
    }

    protected List<ListUnspentTransactionOutputsByAddressRI> GetSpendingTransactions(List<ListUnspentTransactionOutputsByAddressRI> transactions, decimal amount)
    {
        var sortedUnspentTransactions = transactions.OrderByDescending(trx => Convert.ToDouble(trx.Amount)).ToList();
        if (!sortedUnspentTransactions.Any()) return null;
        var spendingCoins = new List<ListUnspentTransactionOutputsByAddressRI>();
        var unspents = transactions.ToArray();
        var sum = 0.0;
        var i = 0;
        do
        {
            var trx = sortedUnspentTransactions[i];
            sum += Convert.ToDouble(trx.Amount);
            spendingCoins.Add(trx);
            i++;
        } while (i < sortedUnspentTransactions.Count && sum <= Convert.ToDouble(amount));

        return spendingCoins;
    }
}