using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IClanChallengeRepository : IRepository<ClanChallenge>
{
    Task<ClanChallenge> GetChallengeWithProgressAsync(Guid challengeId); // Get challenge with progress tracking
    Task<IEnumerable<ClanChallenge>> GetChallengesByClanIdAsync(Guid clanId); // Get challenges for a specific clan
}