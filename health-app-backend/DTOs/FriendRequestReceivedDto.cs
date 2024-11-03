namespace health_app_backend.DTOs;

public class FriendRequestReceivedDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderUsername { get; set; }
    public Guid ReceiverId { get; set; }
    public DateTime ReceivedAt { get; set; }
    
}