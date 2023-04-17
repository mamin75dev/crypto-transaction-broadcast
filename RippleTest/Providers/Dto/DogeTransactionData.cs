namespace RippleTest.Providers.Dto;

public class DogeTransactionData
{
    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Total { get; set; }
    public List<DogeTransaction> Items { get; set; }
}