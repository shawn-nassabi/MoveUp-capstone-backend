namespace health_app_backend.Models;

public class Friend
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid FriendId { get; set; }
    public User FriendUser { get; set; }
    public DateTime FriendshipStartedAt { get; set; } = DateTime.UtcNow;
}