using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Ripple.Binary.Codec.Enums;
using Ripple.Binary.Codec.Types;
using Ripple.Keypairs.K256;
using Ripple.Signing.Utils;
using RippleLibSharp.Keys;
using RippleTest.Classes;
using Xrpl.Client;
using Xrpl.Client.Model.Transaction.Interfaces;
using Xrpl.Client.Model.Transaction.TransactionTypes;
using Xrpl.Client.Requests.Transaction;
using Xrpl.Wallet;
using Currency = Xrpl.Client.Model.Currency;

namespace RippleTest.Controllers;

[ApiController]
public class RippleController : ControllerBase
{
    private static readonly string
        privateKeyString =
            "5d70e7828537bbc145a9bd0af020b689eec6f4f05393b2fd1f97645e4dda539e"; // prv key with extra little with xrpl.js

    private readonly IRippleService _service;

    public RippleController(IRippleService service)
    {
        _service = service;
    }

    [HttpGet("publish")]
    public string PublishTransaction()
    {
        
        
        var pk = new RipplePublicKey("02fc3e621446ff3cbb91ae6509ee96a01b824d9bdf20f281e91a602800c69bfdca"
            .FromHexToByteArray());

        var address = pk.GetAddress();

        return address;

        //
        //
        //
        // var prv = new BigInteger("5B543B634038C89E7869F75B49F004C59ECF0E106281E35FD68E1E564A862394", 16);
        // // var prv = new BigInteger("5d70e7828537bbc145a9bd0af020b689eec6f4f05393b2fd1f97645e4dda539e", 16);
        // IKeyPair keyPair = new K256KeyPair(prv);
        // var signer = TxSigner.FromKeyPair(keyPair);
        // var client = new RippleClient("wss://s.altnet.rippletest.net:51233");
        // client.Connect();
        //
        // var info = await client.AccountInfo("rJfxe2uh3KPef9LGE64FAN72r5AQVn4Z24");
        //
        // IPaymentTransaction trx = new PaymentTransaction
        // {
        //     Destination = "rJfxe2uh3KPef9LGE64FAN72r5AQVn4Z24",
        //     Account = "rXEPkZ5hbU1MfF4QiaNbeQBQrfCeMGbBu",
        //     Amount = new Currency { ValueAsXrp = 1 },
        //     Fee = new Currency { Value = "12" },
        //     Sequence = info.AccountData.Sequence
        // };
        //
        // // var stTransaction = StObject.FromJson(JObject.Parse(trx.ToJson()));
        //
        // // stTransaction.SetFlag(2147483648U);
        // // stTransaction[Field.SigningPubKey] = keyPair.CanonicalPubBytes();
        // // stTransaction.SetFlag(2147483648U);
        //
        // // var hash = RippleHashUtils.TransactionId(stTransaction.ToBytes());
        //
        // // _service.SaveTransaction(stTransaction);
        // // return hash;
        // // TODO: sign the transaction hash
        // // stTransaction[Field.TxnSignature] = "sign".FromHexToByteArray();
        //
        //
        // // return "result:::" + stTransaction.SigningData().ToHexString();
        //
        // // return stTransaction.SigningData().ToHexString();
        //
        // // stTransaction.SetFlag(2147483648U);
        // var signed = signer.SignJson(JObject.Parse(trx.ToJson()));
        // //
        // //
        // // var signed = TxSigner.ValidateAndEncode(stTransaction);
        // //
        // var req = new SubmitBlobRequest
        // {
        //     TransactionBlob = signed.TxBlob
        // };
        //
        // return signed.TxBlob;
        // try
        // {
        //     var result = await client.SubmitTransactionBlob(req);
        //     Console.WriteLine("result:    " + result.EngineResultMessage);
        //     return "result:::   " + result.Transaction.Hash;
        //
        //     // return signed.TxBlob;
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine("error:  " + e.Message);
        //     return "error:::  " + e.Message;
        // }
    }

    [HttpGet("unspent")]
    public async Task<string> UnspentTransaction()
    {
        var client = new RippleClient("wss://s.altnet.rippletest.net:51233");
        client.Connect();

        var info = await client.AccountInfo("rXEPkZ5hbU1MfF4QiaNbeQBQrfCeMGbBu");

        IPaymentTransaction trx = new PaymentTransaction
        {
            Destination = "rJfxe2uh3KPef9LGE64FAN72r5AQVn4Z24",
            Account = "rXEPkZ5hbU1MfF4QiaNbeQBQrfCeMGbBu",
            Amount = new Currency { ValueAsXrp = 1 },
            Fee = new Currency { Value = "12" },
            Sequence = info.AccountData.Sequence
        };

        var stTransaction = StObject.FromJson(JObject.Parse(trx.ToJson()), true);

        stTransaction.SetFlag(2147483648U);
        stTransaction[Field.SigningPubKey] =
            "02fc3e621446ff3cbb91ae6509ee96a01b824d9bdf20f281e91a602800c69bfdca".FromHexToByteArray();

        var hash = Sha512.Half(stTransaction.SigningData()).ToHexString();

        _service.SaveTransaction(stTransaction);
        return hash;
    }

    [HttpGet("spend")]
    public async Task<string> SpendTransaction(string signature)
    {
        var tx = _service.SpendTransaction();
        var rs = SignatureUtils.ExtractRSFromSignature(signature);
        var sign = new EcdsaSignature(rs[0], rs[1]);

        tx[Field.TxnSignature] = sign.EncodeToDer();

        var signed = TxSigner.ValidateAndEncode(tx);

        var req = new SubmitBlobRequest
        {
            TransactionBlob = signed.TxBlob
        };
        var client = new RippleClient("wss://s.altnet.rippletest.net:51233");
        client.Connect();

        try
        {
            var result = await client.SubmitTransactionBlob(req);
            client.Disconnect();
            return "result:::   " + result.EngineResultMessage;
        }
        catch (Exception e)
        {
            client.Disconnect();
            return "error:::  " + e.Message;
        }
    }
}