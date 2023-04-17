using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using NBitcoin.Altcoins;
using RippleTest.Dto;

namespace RippleTest.Controllers;

[ApiController]
public class DogeController : ControllerBase
{
    private readonly IDogeService _service;

    public DogeController(IDogeService service)
    {
        _service = service;
    }

    [HttpPost]
    [Route("unspent")]
    public async Task<ApiResponse> SaveTransaction(UnspentRequestDto request)
    {
        var network = Dogecoin.Instance.Testnet;
        var unspentResponseDto = await _service.SaveTransaction(request, network);

        return new ApiResponse(unspentResponseDto);
    }

    [HttpPost]
    [Route("spend")]
    public async Task<ApiResponse> SpendTransaction(SpendRequestDto request)
    {
        try
        {
            var spendResponseDto = await _service.SpendTransaction(request);

            return new ApiResponse(spendResponseDto);
        }
        catch (Exception e)
        {
            return new ApiResponse("spend error");
        }
    }
}