namespace RippleTest.Providers.Dto
{
    public class DogeTransactionDetailsDataItem
    {
        public bool IsConfirmed { get; set; }
        public string TransactionID { get; set; }
        public int Index { get; set; }
        public string MinedInBlockHash { get; set; }
        public long MinedInBlockHeight { get; set; }
        public List<DogeTransactionContributer> Recipients { get; set; }
        public List<DogeTransactionContributer> Senders { get; set; }
        public long Timestamp { get; set; }
        public string TransactionHash { get; set; }
        public BlockchainSpecific BlockchainSpecific { get; set; }
        public DogeTransactionFee Fee { get; set; }
    }
}
