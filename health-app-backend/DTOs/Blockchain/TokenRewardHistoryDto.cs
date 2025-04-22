namespace health_app_backend.DTOs;

public class TokenRewardHistoryDto
{
    public Guid Id { get; set; }
    public string WalletAddress { get; set; }
    public long Tokens { get; set; }
    public DateTime Timestamp { get; set; }
}