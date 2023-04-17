using NBitcoin;

namespace RippleTest.Dto;

public class UnspentResponseDto
{
    public ScriptPubKeyType ScriptPubKeyType { get; set; }
    public string SenderAddress { get; set; }
    public string? TransactionHash { get; set; }
    public string VerificationHash { get; set; }
}