using System.Numerics;

namespace health_app_backend.Models;

public class PointsRewardHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string WalletAddress { get; set; }
    public BigInteger Points { get; set; }
    public DateTime Timestamp { get; set; }
}