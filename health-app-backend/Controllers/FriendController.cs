using health_app_backend.DTOs;
using health_app_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers;

[ApiController]
[Route("api/friends")]
public class FriendController : Controller
{
    private readonly IFriendService _friendService;

    public FriendController(IFriendService friendService)
    {
        _friendService = friendService;
    }

    [HttpPost("send-request")]
    public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequestDto friendRequestDto)
    {
        string result = await _friendService.SendFriendRequest(friendRequestDto.SenderId, friendRequestDto.ReceiverId);
        return Ok(result);
    }

    [HttpPost("accept-request/{requestId}")]
    public async Task<IActionResult> AcceptFriendRequest(Guid requestId)
    {
        await _friendService.AcceptFriendRequest(requestId);
        return Ok("Friend request accepted.");
    }

    [HttpGet("requests/{userId}")]
    public async Task<ActionResult<IEnumerable<FriendRequestReceivedDto>>> GetFriendRequests(Guid userId)
    {
        try
        {
            var friendRequests = await _friendService.GetFriendRequests(userId);
            return Ok(friendRequests);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetFriends(Guid userId)
    {
        var friends = await _friendService.GetFriendsAsync(userId);
        return Ok(friends);
    }

    [HttpGet("{friendId}/activity")]
    public async Task<ActionResult<IEnumerable<HealthDataResponseDto>>> GetFriendActivity(Guid friendId)
    {
        var activity = await _friendService.GetFriendRecentActivity(friendId);
        return Ok(activity);
    }
}