using health_app_backend.DTOs;
using health_app_backend.Models;

namespace health_app_backend.Services;

public interface IUserBenchmarkRecordService
{
    public Task<IEnumerable<UserBenchmarkResponseDto>> GetUserBenchmarkRecordsByUserIdAsync(Guid userId);
}