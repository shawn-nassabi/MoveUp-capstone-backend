using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IDemographicBenchmarkRepository : IRepository<DemographicBenchmark>
{
    IQueryable<DemographicBenchmark> GetAll();
    IQueryable<DemographicBenchmark> GetAllByUserId(Guid userId);
}