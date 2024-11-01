using AutoMapper;
using health_app_backend.DTOs;
using health_app_backend.Models;
using health_app_backend.Repositories;

namespace health_app_backend.Services;

public class FriendService : IFriendService
{
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly IFriendRepository _friendRepository;
    private readonly IHealthDataRepository _healthDataRepository;
    private readonly IMapper _mapper;

    public FriendService(IFriendRequestRepository friendRequestRepository, IFriendRepository friendRepository, IHealthDataRepository healthDataRepository, IMapper mapper)
    {
        _friendRequestRepository = friendRequestRepository;
        _friendRepository = friendRepository;
        _healthDataRepository = healthDataRepository;
        _mapper = mapper;
    }

    public async Task SendFriendRequest(Guid senderId, Guid receiverId)
    {
        await _friendRequestRepository.SendFriendRequestAsync(senderId, receiverId);
    }

    public async Task AcceptFriendRequest(Guid requestId)
    {
        await _friendRequestRepository.AcceptFriendRequestAsync(requestId);
    }

    public async Task<IEnumerable<UserResponseDto>> GetFriendsAsync(Guid userId)
    {
        var friends = await _friendRepository.GetFriendsAsync(userId);
        return _mapper.Map<IEnumerable<UserResponseDto>>(friends);
    }

    public async Task<IEnumerable<HealthDataResponseDto>> GetFriendRecentActivity(Guid friendId)
    {
        DateTime oneWeekAgo = DateTime.UtcNow.AddDays(-7);
        DateTime now = DateTime.UtcNow;
        var healthData = await _healthDataRepository.GetFriendActivityAsync(friendId, oneWeekAgo, now);
        return _mapper.Map<IEnumerable<HealthDataResponseDto>>(healthData);
    }
}