using health_app_backend.DTOs;
using health_app_backend.Models;

namespace health_app_backend.Services;

public interface IFriendService
{
    Task<String> SendFriendRequest(Guid senderId, Guid receiverId);
    Task<IEnumerable<FriendRequestReceivedDto>> GetFriendRequests(Guid userId);
    Task AcceptFriendRequest(Guid requestId);
    Task DeclineFriendRequest(Guid requestId);
    Task<bool> DeleteFriend(Guid userId, Guid friendId);
    Task<IEnumerable<UserResponseDto>> GetFriendsAsync(Guid userId);
    Task<IEnumerable<HealthDataResponseDto>> GetFriendRecentActivity(Guid friendId);
}