namespace RippleTest.Providers.Dto
{
    public class DogeTransactionOutputScriptPubKey
    {
        public string[] Addresses { get; set; }
        public string ASM { get; set; }
        public string HEX { get; set; }
        public int RegSigs { get; set; }
        public string Type { get; set; }
    }
}
