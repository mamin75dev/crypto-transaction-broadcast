namespace RippleTest.Providers.Dto;

public class DogeTransaction
{
    public string Address { get; set; }
    public string Amount { get; set; }
    public int Index { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsConfirmed { get; set; }
    public long Timestamp { get; set; }
    public string TransactionId { get; set; }
}