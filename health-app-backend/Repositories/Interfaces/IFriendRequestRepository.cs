using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IFriendRequestRepository : IRepository<FriendRequest>
{
    Task<String> SendFriendRequestAsync(Guid senderId, Guid receiverId);
    Task AcceptFriendRequestAsync(Guid requestId);
    Task DeclineFriendRequestAsync(Guid requestId);
    Task<IEnumerable<FriendRequest>> GetPendingRequestsAsync(Guid userId);
}