using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using health_app_backend.Models;

namespace health_app_backend.Repositories
{
    public interface IHealthDataRepository : IRepository<HealthData>
    {
        
        Task<IEnumerable<HealthData>> GetAllByUsernameAsync(string username);
        Task<IEnumerable<HealthData>> GetAllByUserIdAsync(Guid userId);
        Task<IEnumerable<HealthData>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate);
        IQueryable<HealthData> GetAll();
        Task<IEnumerable<HealthData>> GetFriendActivityAsync(Guid friendId, DateTime fromDate, DateTime toDate);
    }
}