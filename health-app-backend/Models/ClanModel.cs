namespace health_app_backend.Models;

public class Clan
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public float ChallengePoints { get; set; } = 0.0f;

    // Relationship to leader
    public Guid LeaderId { get; set; }
    public User Leader { get; set; }

    // Navigation property for members
    public ICollection<ClanMember> Members { get; set; } = new List<ClanMember>();

    // Navigation property for challenges
    public ICollection<ClanChallenge> Challenges { get; set; } = new List<ClanChallenge>();
}