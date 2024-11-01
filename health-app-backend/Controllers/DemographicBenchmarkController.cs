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
        private readonly IUserBenchmarkRecordService _userBenchmarkService;

        public DemographicBenchmarkController(IDemographicBenchmarkService demographicBenchmarkService, IUserBenchmarkRecordService userBenchmarkRecordService)
        {
            _demographicBenchmarkService = demographicBenchmarkService;
            _userBenchmarkService = userBenchmarkRecordService;
        }
        
        // Get all benchmarks for a specific userId
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<UserBenchmarkResponseDto>>> GetDemographicBenchmark(Guid userId)
        {
            try
            {
                var benchmarks = await _userBenchmarkService.GetUserBenchmarkRecordsByUserIdAsync(userId);
                if (benchmarks == null)
                {
                    return NotFound("User not found");
                }
                return Ok(benchmarks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ex.Message);
                throw;
            }
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
