namespace health_app_backend.DTOs;

public class ClanChallengeDto
{
    public string Id { get; set; }
    public string ClanId { get; set; }
    public string ChallengeName { get; set; }
    public string ChallengeDescription { get; set; }
    public string DataType { get; set; }
    public float Goal { get; set; }
    public bool IsCompleted { get; set; }
    public float TotalProgress { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}