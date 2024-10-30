using health_app_backend.Models;

namespace health_app_backend.Repositories;

public class UserBenchmarkRecordRepository : Repository<UserBenchmarkRecord>
{
    public UserBenchmarkRecordRepository (AppDbContext context) : base(context) { }
}