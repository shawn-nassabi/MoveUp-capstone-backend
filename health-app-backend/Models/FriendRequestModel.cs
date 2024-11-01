namespace health_app_backend.Models;

public class FriendRequest
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public User Sender { get; set; }
    public Guid ReceiverId { get; set; }
    public User Receiver { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsAccepted { get; set; } = false;
    public bool IsPending { get; set; } = true;
}