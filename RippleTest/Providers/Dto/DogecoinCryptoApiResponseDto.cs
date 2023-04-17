namespace RippleTest.Providers.Dto;

public class DogecoinCryptoApiResponseDto
{
    private string ApiVersion { get; set; }
    public string RequestId { get; set; }
    public DogeTransactionData Data { get; set; }
}