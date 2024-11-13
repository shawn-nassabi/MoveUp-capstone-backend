using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class ClanChallengeRepository : Repository<ClanChallenge>, IClanChallengeRepository
{
    private readonly AppDbContext _context;

    public ClanChallengeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<ClanChallenge> GetChallengeWithProgressAsync(Guid challengeId)
    {
        return await _context.ClanChallenges
            .Include(challenge => challenge.Progress)
            .ThenInclude(progress => progress.ClanMember) // Include member details for each progress entry
            .FirstOrDefaultAsync(challenge => challenge.Id == challengeId);
    }

    public async Task<IEnumerable<ClanChallenge>> GetChallengesByClanIdAsync(Guid clanId)
    {
        return await _context.ClanChallenges
            .Where(challenge => challenge.ClanId == clanId)
            .ToListAsync();
    }
}