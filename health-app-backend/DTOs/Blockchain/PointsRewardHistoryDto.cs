namespace health_app_backend.DTOs;

public class PointsRewardHistoryDto
{
    public Guid Id { get; set; }
    public string WalletAddress { get; set; }
    public long Points { get; set; }
    public DateTime Timestamp { get; set; }
}