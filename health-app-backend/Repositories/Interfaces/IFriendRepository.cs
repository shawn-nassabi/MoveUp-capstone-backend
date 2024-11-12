using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IFriendRepository : IRepository<Friend>
{
    Task<IEnumerable<User>> GetFriendsAsync(Guid userId);
    Task<bool> DeleteFriendAsync(Guid userId, Guid friendId);
}