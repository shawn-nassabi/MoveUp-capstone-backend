using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class PointsRewardHistoryRepository : Repository<PointsRewardHistory>, IPointsRewardHistoryRepository
{
    private readonly AppDbContext _context;
    public IUnitOfWork UnitOfWork => _context;    // expose your context as unit of work
    
    public PointsRewardHistoryRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<PointsRewardHistory>> GetByWalletAddressAsync(string walletAddress)
    {
        return await _context.PointsRewardHistories
            .Where(p => p.WalletAddress.ToLower() == walletAddress.ToLower())
            .OrderByDescending(p => p.Timestamp)
            .ToListAsync();
    }
}