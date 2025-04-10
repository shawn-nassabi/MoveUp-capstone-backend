using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace health_app_backend.FunctionMessageClasses
{
    [Event("TokensMinted")]
    public class TokensMintedEventDTO : IEventDTO
    {
        [Parameter("address", "user", 1, true)]
        public string User { get; set; }

        [Parameter("uint256", "tokens", 2, false)]
        public BigInteger Tokens { get; set; }
    }
}