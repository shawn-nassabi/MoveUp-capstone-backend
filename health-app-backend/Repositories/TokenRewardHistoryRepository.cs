using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class TokenRewardHistoryRepository : Repository<TokenRewardHistory>, ITokenRewardHistoryRepository
{
    private readonly AppDbContext _context;
    public IUnitOfWork UnitOfWork => _context;    // expose your context as unit of work

    public TokenRewardHistoryRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TokenRewardHistory>> GetByWalletAddressAsync(string walletAddress)
    {
        return await _context.TokenRewardHistories
            .Where(t => t.WalletAddress.ToLower() == walletAddress.ToLower())
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();
    }
}