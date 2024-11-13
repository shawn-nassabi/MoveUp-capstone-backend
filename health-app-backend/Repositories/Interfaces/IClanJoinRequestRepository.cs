using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IClanJoinRequestRepository : IRepository<ClanJoinRequest>
{
    Task<IEnumerable<ClanJoinRequest>> GetPendingRequestsForClanAsync(Guid clanId); // Retrieve all pending join requests for a clan
    Task<IEnumerable<ClanJoinRequest>> GetPendingRequestsForUserAsync(Guid userId); // Retrieve all pending join requests for a user
    Task<bool> ApproveJoinRequestAsync(Guid requestId); // Approve a specific join request
    Task<bool> RejectJoinRequestAsync(Guid requestId); // Reject a specific join request
}