using System.Threading.Tasks;
using health_app_backend.DTOs;
using health_app_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers
{
    [ApiController]
    [Route("api/demographicbenchmark")]
    public class DemographicBenchmarkController : ControllerBase
    {
        private readonly IDemographicBenchmarkService _demographicBenchmarkService;

        public DemographicBenchmarkController(IDemographicBenchmarkService demographicBenchmarkService)
        {
            _demographicBenchmarkService = demographicBenchmarkService;
        }
        
        [HttpPost]
        public async Task<ActionResult<UserBenchmarkResponseDto>> GetBenchmark(UserBenchmarkCreateDto benchmarkCreateDto)
        {
            try
            {
                var benchmark = await _demographicBenchmarkService.GetOrCreateBenchmarkAsync(benchmarkCreateDto);
                return Ok(benchmark);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }
    }
}
