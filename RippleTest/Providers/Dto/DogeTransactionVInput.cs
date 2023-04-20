namespace RippleTest.Providers.Dto
{
    public class DogeTransactionVInput
    {
        public string[] Addresses { get; set; }
        public DogeTransactionInputScriptSig ScriptSig { get; set; }
        public Int64 Sequence { get; set; }
        public string TxID { get; set; }
        public object[] TxInWitness { get; set; }
        public string Value { get; set; }
        public int Vout { get; set; }
    }
}
