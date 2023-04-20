namespace RippleTest.Providers.Dto
{
    public class DogeTransactionVOutput
    {
        public bool IsSpent { get; set; }
        public DogeTransactionOutputScriptPubKey ScriptPubKey { get; set; }
        public string Value { get; set; }
    }
}
