namespace RippleTest.Providers.Dto
{
    public class BlockchainSpecific
    {
        public long LockTime { get; set; }
        public int Size { get; set; }
        public int Version { get; set; }
        public List<DogeTransactionVInput> Vin { get; set; }
        public List<DogeTransactionVOutput> Vout { get; set; }

    }
}
