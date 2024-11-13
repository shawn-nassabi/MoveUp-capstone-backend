using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IClanMemberRepository : IRepository<ClanMember>
{
    public Task<IEnumerable<ClanMember>> GetAllByClanId(Guid clanId);
    public new Task<ClanMember> GetByIdAsync(Guid memberId);
    public Task<ClanMember> GetByUserIdAsync(Guid userId);
    Task<bool> RemoveMemberFromClanAsync(Guid clanId, Guid userId);
}