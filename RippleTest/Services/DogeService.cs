using NBitcoin;
using NBitcoin.Crypto;
using RippleTest.Broadcast;
using RippleTest.Dto;
using RippleTest.Providers;
using RippleTest.Utils;

namespace RippleTest;

public class DogeService : IDogeService
{
    private readonly ICryptoApiService _cryptoService;
    private readonly TransactionDictionary _dictionary;
    public DogeService(ICryptoApiService cryptoService, TransactionDictionary dictionary)
    {
        _cryptoService = cryptoService;
        _dictionary = dictionary;
    }

    public async Task<UnspentResponseDto> SaveTransaction(UnspentRequestDto request, Network network)
    {
        var scriptType = ScriptPubKeyType.Legacy;
        var dogeSenderPublicKey = new PubKey(request.SenderPublicKey);
        var destination = BitcoinAddress.Create(request.ReceiverAddress, network);
        var sender = dogeSenderPublicKey.GetAddress(scriptType, network).ToString();

        var senderChangeAddress = BitcoinAddress.Create(sender, network);

        var result = await _cryptoService.GetUnspentCoins(sender, request.SenderPublicKey);


        var spendingCoins = await _cryptoService.GetSpendingCoins(sender, request.AmountInDoge);

        var responseDto = new UnspentResponseDto
        {
            ScriptPubKeyType = scriptType,
            SenderAddress = sender,
            VerificationHash = HashGenerator.DoubleSha256($"{request.ReceiverAddress}{request.AmountInDoge}")
        };

        if (spendingCoins.Count == 0) return responseDto;

        var unsignedTrx = network
            .CreateTransactionBuilder()
            .AddCoins(spendingCoins)
            .Send(destination.ScriptPubKey, Money.Coins(request.AmountInDoge))
            .SendEstimatedFees(new FeeRate(request.SatoshiPerByte))
            .SetChange(senderChangeAddress)
            .BuildTransaction(false);

        var rebuild = network.CreateTransactionBuilder();
        rebuild.AddCoins(spendingCoins);

        for (int i = 0; i < spendingCoins.Count; i++)
        {
            var coin = spendingCoins[i];
            var indexedIn = unsignedTrx.Inputs.FindIndexedInput(coin.Outpoint);
            if (indexedIn == null) continue;
            var signHash = indexedIn.GetSignatureHash(coin, SigHash.All);
            var reverseHash = Convert.ToHexString(signHash.ToBytes(true));

            _dictionary.AddTransactionCachedData(request.SenderPublicKey, reverseHash, spendingCoins, unsignedTrx, rebuild, i, sender, request.ReceiverAddress, request.AmountInDoge, "doge", "doge");

            responseDto.TransactionHash = reverseHash;
            return responseDto;
        }

        // return unsignedTrx.ToHex();

        return responseDto;
    }

    public async Task<SpendResponseDto> SpendTransaction(SpendRequestDto request)
    {
        try
        {
            TransactionCachedData? cachedData = _dictionary.GetTransactionCachedData(request.Hash);
            if (cachedData == null)
            {
                // _dictionary.RemoveFromDictionary($"{request.Hash}");
                return new SpendResponseDto(true, errors: "Transaction does not exists!!");
            }

            AddSignatureToTransaction(request, cachedData);

            for (int i = cachedData.Index + 1; i < cachedData.UnspentCoins?.Count; i++)
            {
                var indexedIn = cachedData.UnsignedTransaction?.Inputs.FindIndexedInput(cachedData.UnspentCoins[i].Outpoint);
                if (indexedIn == null) continue;
                string? reverseHash;
                var signHash = indexedIn.GetSignatureHash(cachedData.UnspentCoins[i], SigHash.All);
                reverseHash = Convert.ToHexString(signHash.ToBytes(true));
                _dictionary.AddTransactionCachedData(cachedData.PublicKey, reverseHash, cachedData.UnspentCoins, cachedData.UnsignedTransaction, cachedData.Rebuild, i, cachedData.From, cachedData.To, cachedData.Value, cachedData.Asset, cachedData.Net);
                // _dictionary.RemoveFromDictionary($"{request.Hash}");
                return new SpendResponseDto(false, reverseHash);
            }
            var spendResponseDto = await VerifyAndBroadcastTransaction(cachedData, cachedData.UnsignedTransaction);
            /*if (!string.IsNullOrEmpty(spendResponseDto.Errors))
            {
                return spendResponseDto;
            }*/
            // await _pendingTransactionManager.AddTransactionAsync(cachedData.From, cachedData.To,
            //     spendResponseDto.Hash, cachedData.Asset, cachedData.Net,
            //     cachedData.NetType, cachedData.Value, DateTime.Now.ToUnixTimestamp());
            // _dictionary.RemoveFromDictionary($"{request.Hash}");
            return spendResponseDto;
        }
        catch (Exception ex)
        {
            _dictionary.RemoveFromDictionary($"{request.Hash}");
            return new SpendResponseDto(true, errors: ex.Message);
        }
    }

    private async Task<SpendResponseDto> VerifyAndBroadcastTransaction(TransactionCachedData cachedData, Transaction? unsignedTransaction)
    {
        var signedTx = cachedData.Rebuild.SignTransaction(unsignedTransaction);

        var verify = cachedData.Rebuild.Verify(signedTx);
        if (!verify)
        {
            return new SpendResponseDto(true, errors: "transaction not verified");
        }

        return new SpendResponseDto(true, signedTx.ToHex(), "");
    }



    protected void AddSignatureToTransaction(SpendRequestDto dto, TransactionCachedData cachedData)
    {
        var signedCoin = cachedData.UnspentCoins?[cachedData.Index];
        ECDSASignature sig =
            new ECDSASignature(
                Convert.FromHexString(SmartCardHelper.ParseJavacardSignatureAndRemoveLeadingZeros(dto.Sign))).MakeCanonical();
        var trxSignature = new TransactionSignature(sig, SigHash.All).MakeCanonical();
        cachedData.Rebuild.AddKnownSignature(new PubKey(cachedData.PublicKey), trxSignature, signedCoin.Outpoint);
    }
}