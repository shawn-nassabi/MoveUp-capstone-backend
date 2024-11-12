namespace health_app_backend.Models;

public class ClanJoinRequest
{
    public Guid Id { get; set; }
    public Guid ClanId { get; set; }
    public Clan Clan { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = false;
    public bool IsPending { get; set; } = true;
}