using health_app_backend.Models;

namespace health_app_backend.Repositories;

public class DemographicBenchmarkRepository : Repository<DemographicBenchmark>
{
    public DemographicBenchmarkRepository(AppDbContext context) : base(context) { }
}