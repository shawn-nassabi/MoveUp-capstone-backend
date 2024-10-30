using System.Linq;
using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories
{
    public class DemographicBenchmarkRepository : Repository<DemographicBenchmark>, IDemographicBenchmarkRepository
    {
        private readonly AppDbContext _context;

        public DemographicBenchmarkRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Returns an IQueryable<DemographicBenchmark> for further filtering
        public IQueryable<DemographicBenchmark> GetAll()
        {
            return _context.DemographicBenchmarks
                .Include(b => b.Location)    // Eager load Location
                .Include(b => b.DataType);   // Eager load DataType
        }
    }
}