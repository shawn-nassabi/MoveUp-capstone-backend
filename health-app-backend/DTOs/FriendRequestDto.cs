namespace health_app_backend.DTOs;

public class FriendRequestDto
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
}