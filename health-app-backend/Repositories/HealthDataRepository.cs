using health_app_backend.Models;

namespace health_app_backend.Repositories;

public class HealthDataRepository : Repository<HealthData>
{
    public HealthDataRepository(AppDbContext context) : base(context) { }
}