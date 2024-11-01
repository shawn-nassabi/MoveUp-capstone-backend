using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories
{
    public class HealthDataRepository : Repository<HealthData>, IHealthDataRepository
    {
        private readonly AppDbContext _context;

        public HealthDataRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Override GetByIdAsync to include User and DataType
        public async Task<HealthData> GetByIdAsync(Guid id)
        {
            return await _context.HealthData
                .Include(hd => hd.User)      // Eager load User
                .Include(hd => hd.Datatype)  // Eager load DataType
                .FirstOrDefaultAsync(hd => hd.Id == id);
        }

        // Get all HealthData by Username
        public async Task<IEnumerable<HealthData>> GetAllByUsernameAsync(string username)
        {
            // Find the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return Enumerable.Empty<HealthData>(); // Return an empty list if user not found
            }

            // Get all HealthData entries for the user and include User and DataType entities
            return await _context.HealthData
                .Where(hd => hd.UserId == user.Id)
                .Include(hd => hd.User)       // Eager load User
                .Include(hd => hd.Datatype)   // Eager load DataType
                .ToListAsync();
        }

        public async Task<IEnumerable<HealthData>> GetAllByUserIdAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Enumerable.Empty<HealthData>(); // Return an empty list if user not found
            }
            // Get all HealthData entries for the user and include User and DataType entities
            return  _context.HealthData
                .Where(hd => hd.UserId == userId)
                .Include(hd => hd.Datatype)
                .Include(hd => hd.User)
                .AsEnumerable();

        }
        
        // Get HealthData for a specific user within a date range
        public async Task<IEnumerable<HealthData>> GetAllByUserIdAndDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate)
        {
            return await _context.HealthData
                .Where(hd => hd.UserId == userId && hd.RecordedAt >= fromDate && hd.RecordedAt <= toDate)
                .Include(hd => hd.User)       // Eager load User
                .Include(hd => hd.Datatype)   // Eager load DataType
                .ToListAsync();
        }

        // Similarly, for GetAllAsync, include related entities
        public  async Task<IEnumerable<HealthData>> GetAllAsync()
        {
            return await _context.HealthData
                .Include(hd => hd.User)      // Eager load User
                .Include(hd => hd.Datatype)  // Eager load DataType
                .ToListAsync();
        }
        
        public IQueryable<HealthData> GetAll()
        {
            return _context.HealthData
                .Include(hd => hd.User) // Eager load User
                .Include(hd => hd.Datatype); // Eager load DataType
        }
        
        // Get friend activity for a specified date range
        public async Task<IEnumerable<HealthData>> GetFriendActivityAsync(Guid friendId, DateTime fromDate, DateTime toDate)
        {
            return await _context.HealthData
                .Where(hd => hd.UserId == friendId && hd.RecordedAt >= fromDate &&  hd.RecordedAt <= toDate)
                .ToListAsync();
        }

    }
}
