namespace health_app_backend.Models;

public class ClanMember
{
    public Guid Id { get; set; }
    public Guid ClanId { get; set; }
    public Clan Clan { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Role in the clan (e.g., member, co-leader)
    public string Role { get; set; } = "Member"; // "Member" or "Leader"
}