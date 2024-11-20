using health_app_backend.DTOs;

namespace health_app_backend.Services;

public interface IClanService
{
    Task<string> CreateClan(ClanCreateDto details);
    Task<IEnumerable<ClanSearchDto>> GetClansAsync();
    Task<ClanDetailsDto> GetClanDetailsAsync(string clanId);
    Task<bool> SendClanInvite(string clanId, string userId);
    Task<bool> AcceptClanInvite(string requestId);
    Task<bool> DeclineClanInvite(string requestId);
    Task<IEnumerable<ClanInviteDto>> GetClanInvites(string clanId);
    Task<IEnumerable<ClanChallengeDto>> GetClanChallenges(string clanId);
    Task<ClanChallengeDto> CreateClanChallenge(ClanChallengeCreateDto details);
    Task UpdateClanChallengeProgress(Guid clanId, string dataType, float contributionAmount);
}