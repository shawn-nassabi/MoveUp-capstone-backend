namespace health_app_backend.Models;

public class ClanChallenge
{
    public Guid Id { get; set; }
    public Guid ClanId { get; set; }
    public Clan Clan { get; set; }

    public string ChallengeName { get; set; } // e.g., "Step Challenge"
    public string Description { get; set; } // e.g., "Perform 50,000 steps collectively"
    public string DataType { get; set; } // e.g., "steps" or "calories"

    public float Goal { get; set; } // e.g., 50000 for steps
    public bool IsCompleted { get; set; } = false;
    public float TotalProgress { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Navigation property for tracking progress
    public ICollection<ClanChallengeProgress> Progress { get; set; } = new List<ClanChallengeProgress>();
}