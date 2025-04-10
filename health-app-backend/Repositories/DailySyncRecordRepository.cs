using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class DailySyncRecordRepository : Repository<DailySyncRecord>, IDailySyncRecordRepository
{
    private readonly AppDbContext _context;

    public DailySyncRecordRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    // Check if data for a given date has already been syned with the blockchain
    public async Task<bool> IsSynced(Guid userId, DateTime date)
    {
        return await _context.DailySyncRecords
            .AnyAsync(r => r.UserId == userId && r.Date == date && r.SyncedOnChain);
    }
    
    // Update the table to mark that data has been synced on the given date
    public async Task SetSynced(Guid userId, DateTime date)
    {
        var record = new DailySyncRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = date,
            SyncedOnChain = true,
            SyncedAt = DateTime.UtcNow
        };
        await _context.DailySyncRecords.AddAsync(record);
        await _context.SaveChangesAsync();
    }
}