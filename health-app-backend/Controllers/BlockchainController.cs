using health_app_backend.DTOs;
using health_app_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers;

[ApiController]
[Route("api/blockchain")]
public class BlockchainController : ControllerBase
{
    private readonly IBlockchainService _blockchainService;

    public BlockchainController(IBlockchainService blockchainService)
    {
        _blockchainService = blockchainService;
    }
    
    // Submit Daily Data on Behalf of User
    [HttpPost("submit-daily-data")]
    public async Task<IActionResult> SubmitDailyData([FromBody] DailyDataRequestDto request)
    {
        await _blockchainService.SubmitDailyDataAsync(request.UserAddress, request.DataTypes, request.HasCondition);
        return Ok("Daily Data Submitted!");
    }
}