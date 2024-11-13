using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class ClanChallengeProgressRepository : Repository<ClanChallengeProgress>, IClanChallengeProgressRepository
{
    private readonly AppDbContext _context;

    public ClanChallengeProgressRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClanChallengeProgress>> GetProgressByChallengeIdAsync(Guid challengeId)
    {
        return await _context.ClanChallengeProgresses
            .Where(progress => progress.ClanChallengeId == challengeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClanChallengeProgress>> GetProgressByMemberIdAsync(Guid memberId)
    {
        return await _context.ClanChallengeProgresses
            .Where(progress => progress.ClanMemberId == memberId)
            .ToListAsync();
    }

    public async Task<float> GetTotalProgressForChallengeAsync(Guid challengeId)
    {
        return await _context.ClanChallengeProgresses
            .Where(progress => progress.ClanChallengeId == challengeId)
            .SumAsync(progress => progress.Contribution);
    }
}