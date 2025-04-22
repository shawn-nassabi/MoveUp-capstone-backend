using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace health_app_backend.DTOs
{
    [Event("TokensMinted")]
    public class TokensEarnedEventDTO : IEventDTO
    {
        [Parameter("address", "user", 1, true)]
        public string User { get; set; }

        [Parameter("uint256", "tokens", 2, false)]
        public BigInteger Tokens { get; set; }
    }
}