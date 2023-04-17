using System.ComponentModel.DataAnnotations;

namespace RippleTest.Dto;

public class UnspentRequestDto
{
    [Required] public string SenderPublicKey { get; set; }

    [Required] public string ReceiverAddress { get; set; }

    [Required] public decimal AmountInDoge { get; set; }

    [Required] public decimal SatoshiPerByte { get; set; }
}