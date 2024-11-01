using health_app_backend.DTOs;
using health_app_backend.Models;

namespace health_app_backend.Services;

public interface IFriendService
{
    Task SendFriendRequest(Guid senderId, Guid receiverId);
    Task AcceptFriendRequest(Guid requestId);
    Task<IEnumerable<UserResponseDto>> GetFriendsAsync(Guid userId);
    Task<IEnumerable<HealthDataResponseDto>> GetFriendRecentActivity(Guid friendId);
}