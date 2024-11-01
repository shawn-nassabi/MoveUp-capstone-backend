using health_app_backend.DTOs;

namespace health_app_backend.Services;

public interface IHealthDataService
{
    Task<HealthDataResponseDto> GetHealthDataAsync(Guid healthDataId);
    Task<IEnumerable<HealthDataResponseDto>> GetHealthDataByUserIdAsync(Guid userId);
    Task<IEnumerable<HealthDataResponseDto>> GetHealthDataByUsernameAndFromDateAsync(string username, string fromDate, string toDate);
    Task<IEnumerable<HealthDataResponseDto>> GetHealthDataByUserIdAndTypeAsync(Guid userId, int datatypeId);
    Task<string> AddHealthDataAsync(HealthDataCreateDto healthDataCreateDto);
}
