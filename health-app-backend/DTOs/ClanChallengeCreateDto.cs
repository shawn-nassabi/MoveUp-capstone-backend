namespace health_app_backend.DTOs;

public class ClanChallengeCreateDto
{
    public string ClanId { get; set; }
    public string DataType { get; set; }
    public string ChallengeName { get; set; }
    public string ChallengeDescription { get; set; }
}