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
        public async Task<ActionResult<HealthDataResponseDto>> GetHealthDataById(string healthDataId)
        {
            var healthData = await _healthDataService.GetHealthDataAsync(healthDataId);
            if (healthData == null)
            {
                return NotFound("Health data not found.");
            }
            return Ok(healthData);
        }

        // Get all health data for the person with the given username
        [HttpGet("user/{username}")]
        public async Task<ActionResult<List<HealthDataResponseDto>>> GetHealthDataByUsername(string username)
        {
            try
            {
                var healthDataList = await _healthDataService.GetHealthDataByUsernameAsync(username);
                return Ok(healthDataList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
