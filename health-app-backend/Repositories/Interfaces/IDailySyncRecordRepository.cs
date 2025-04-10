using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IDailySyncRecordRepository : IRepository<DailySyncRecord>
{
    Task<bool> IsSynced(Guid userId, DateTime date);
    Task SetSynced(Guid userId, DateTime date);
}