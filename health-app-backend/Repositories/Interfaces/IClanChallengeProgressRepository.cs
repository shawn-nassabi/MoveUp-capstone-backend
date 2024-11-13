using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IClanChallengeProgressRepository : IRepository<ClanChallengeProgress>
{
    Task<IEnumerable<ClanChallengeProgress>> GetProgressByChallengeIdAsync(Guid challengeId); // Get all progress entries for a specific challenge
    Task<IEnumerable<ClanChallengeProgress>> GetProgressByMemberIdAsync(Guid memberId); // Get all progress entries by a specific member
    Task<float> GetTotalProgressForChallengeAsync(Guid challengeId); // Get total progress for a specific challenge
}