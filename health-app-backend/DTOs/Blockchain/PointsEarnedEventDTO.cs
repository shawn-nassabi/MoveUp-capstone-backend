using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace health_app_backend.DTOs
{
    [Event("PointsEarned")]
    public class PointsEarnedEventDTO : IEventDTO
    {
        [Parameter("address", "user", 1, true)]
        public string User { get; set; }

        [Parameter("uint256", "points", 2, false)]
        public BigInteger Points { get; set; }
    }
}