using health_app_backend.DTOs;

namespace health_app_backend.Services;

public interface IHealthDataService
{
    public Task<HealthDataResponseDto> GetHealthDataAsync(string healthDataId);
    public Task<List<HealthDataResponseDto>> GetHealthDataByUsernameAsync(string username);
    public Task<List<HealthDataResponseDto>> GetHealthDataByUsernameAndFromDateAsync(string username, string fromDate, string toDate);
    public Task<string> AddHealthDataAsync(HealthDataCreateDto healthDataCreateDto);
}