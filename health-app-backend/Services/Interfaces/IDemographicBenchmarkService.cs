using health_app_backend.DTOs;

namespace health_app_backend.Services;

public interface IDemographicBenchmarkService
{
    Task<UserBenchmarkResponseDto> GetOrCreateBenchmarkAsync(UserBenchmarkCreateDto userBenchmarkCreateDto);
    // Task<IEnumerable<UserBenchmarkResponseDto>> GetAllBenchmarksByUserIdAsync(Guid userId);
}