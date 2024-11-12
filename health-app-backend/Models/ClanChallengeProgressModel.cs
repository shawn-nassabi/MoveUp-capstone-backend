namespace health_app_backend.Models;

public class ClanChallengeProgress
{
    public Guid Id { get; set; }
    public Guid ClanChallengeId { get; set; }
    public ClanChallenge ClanChallenge { get; set; }

    public Guid ClanMemberId { get; set; }
    public ClanMember ClanMember { get; set; }

    public float Contribution { get; set; } // e.g., steps contributed by the member
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}