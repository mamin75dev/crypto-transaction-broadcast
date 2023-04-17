using NBitcoin;
using RippleTest.Dto;

namespace RippleTest;

public interface IDogeService
{
    Task<UnspentResponseDto> SaveTransaction(UnspentRequestDto request, Network network);

    Task<SpendResponseDto> SpendTransaction(SpendRequestDto request);
}