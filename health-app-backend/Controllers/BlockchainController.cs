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
    
    // Submit Daily Data on Behalf of User (to generate points)
    [HttpPost("submit-daily-data")]
    public async Task<IActionResult> SubmitDailyData([FromBody] DailyDataRequestDto request)
    {
        try
        {
            await _blockchainService.SubmitDailyDataAsync(request.UserAddress, request.DataTypes, request.HasCondition);
            return Ok("✅ Daily Data Submitted!");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occurred.", details = ex.Message });
        }
    }


    [HttpGet("points/{userAddress}")]
    public async Task<IActionResult> GetPoints(string userAddress)
    {
        try
        {
            var points = await _blockchainService.GetUserPointsAsync(userAddress);
            return Ok(new { userAddress, points = points.ToString() });
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching user points.", details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occurred.", details = ex.Message });
        }
    }
    
    // Convert user's points to HDT tokens
    [HttpPost("convert-points-to-tokens")]
    public async Task<IActionResult> ConvertPointsToTokens([FromBody] ConvertPointsRequestDto request)
    {
        try
        {
            await _blockchainService.ConvertPointsToTokensAsync(request.UserId);
            return Ok("✅ Points converted to tokens successfully!");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new { message = "Blockchain transaction failed.", details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occurred.", details = ex.Message });
        }
    }
    
    
    [HttpGet("points-per-token")]
    public async Task<IActionResult> GetPointsPerToken()
    {
        try
        {
            var pointsPerToken = await _blockchainService.GetPointsPerTokenAsync();
            return Ok(new { pointsPerToken = pointsPerToken.ToString() });
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching pointsPerToken.", details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occurred.", details = ex.Message });
        }
    }
    
    [HttpGet("token-balance/{userId}")]
    public async Task<IActionResult> GetUserTokenBalance(Guid userId)
    {
        try
        {
            var balance = await _blockchainService.GetUserTokenBalanceAsync(userId);
            return Ok(new { userId, balance = balance.ToString("F6") });
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching user token balance.", details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occurred.", details = ex.Message });
        }
    }
}