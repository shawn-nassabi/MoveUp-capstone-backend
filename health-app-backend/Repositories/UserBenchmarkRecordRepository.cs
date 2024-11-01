using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class UserBenchmarkRecordRepository : Repository<UserBenchmarkRecordModel>, IUserBenchmarkRecordRepository
{
    private readonly AppDbContext _context;

    public UserBenchmarkRecordRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    // Method to retrieve all UserBenchmarkRecords by userId
    public async Task<IEnumerable<UserBenchmarkRecordModel>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.UserBenchmarkRecords
            .Where(ubr => ubr.UserId == userId)
            .Include(ubr => ubr.DemographicBenchmark)       // Include related DemographicBenchmark
            .ThenInclude(db => db.Location)                // Include Location in DemographicBenchmark
            .ToListAsync();                                // Execute and return as a list
    }
}