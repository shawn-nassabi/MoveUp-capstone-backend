using health_app_backend.DTOs;
using health_app_backend.Services;
using Microsoft.AspNetCore.Mvc;
using System;

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


    [HttpGet("points/{userId}")]
    public async Task<IActionResult> GetPoints(Guid userId)
    {
        try
        {
            var points = await _blockchainService.GetUserPointsAsync(userId);
            return Ok(new { userId, points = points.ToString() });
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

    // Get points reward history for a user
    [HttpGet("history/points/{userId}")]
    public async Task<IActionResult> GetPointsRewardHistory(Guid userId)
    {
        try
        {
            var history = await _blockchainService.GetPointsRewardHistoryAsync(userId);
            return Ok(history);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching points reward history.", details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occurred.", details = ex.Message });
        }
    }

    // Get token reward history for a user
    [HttpGet("history/tokens/{userId}")]
    public async Task<IActionResult> GetTokenRewardHistory(Guid userId)
    {
        try
        {
            var history = await _blockchainService.GetTokenRewardHistoryAsync(userId);
            return Ok(history);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching token reward history.", details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unexpected error occurred.", details = ex.Message });
        }
    }
}