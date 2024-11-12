using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IUserBenchmarkRecordRepository : IRepository<UserBenchmarkRecordModel>
{
    public Task<IEnumerable<UserBenchmarkRecordModel>> GetAllByUserIdAsync(Guid userId);
}