using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using health_app_backend.DTOs;
using health_app_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers
{
    [ApiController]
    [Route("api/healthdata")]
    public class HealthDataController : ControllerBase
    {
        private readonly IHealthDataService _healthDataService;

        public HealthDataController(IHealthDataService healthDataService)
        {
            _healthDataService = healthDataService;
        }

        // Get health data by health data ID
        [HttpGet("{healthDataId}")]
        public async Task<ActionResult<HealthDataResponseDto>> GetHealthDataById(Guid healthDataId)
        {
            var healthData = await _healthDataService.GetHealthDataAsync(healthDataId);
            if (healthData == null)
            {
                return NotFound("Health data not found.");
            }
            return Ok(healthData);
        }

        // Get all health data for the person with the given username
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<HealthDataResponseDto>>> GetHealthDataByUsername(Guid userId)
        {
            try
            {
                var healthDataList = await _healthDataService.GetHealthDataByUserIdAsync(userId);
                return Ok(healthDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}/{datatypeId}")]
        public async Task<ActionResult<List<HealthDataResponseDto>>> GetHealthDataByUserIdAndDatatypeId(Guid userId,
            int datatypeId)
        {
            try
            {
                var healthDataList = await _healthDataService.GetHealthDataByUserIdAndTypeAsync(userId, datatypeId);
                return Ok(healthDataList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
                throw;
            }
        }

        // Get all health data for the specified username and given date range
        [HttpGet("user/{username}/daterange")]
        public async Task<ActionResult<List<HealthDataResponseDto>>> GetHealthDataByUsernameAndDateRange(
            string username, [FromQuery] string fromDate, [FromQuery] string toDate)
        {
            try
            {
                var healthDataList = await _healthDataService.GetHealthDataByUsernameAndFromDateAsync(username, fromDate, toDate);
                return Ok(healthDataList);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 if date format is invalid
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Create health data
        [HttpPost]
        public async Task<ActionResult<string>> CreateHealthData(HealthDataCreateDto healthDataCreateDto)
        {
            try
            {
                var healthDataId = await _healthDataService.AddHealthDataAsync(healthDataCreateDto);
                return CreatedAtAction(nameof(GetHealthDataById), new { healthDataId = healthDataId }, healthDataId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
