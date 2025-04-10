using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace health_app_backend.FunctionMessageClasses
{
    [Function("mint")]
    public class MintFunction : FunctionMessage
    {
        [Parameter("address", "to", 1)]
        public string To { get; set; }

        [Parameter("uint256", "amount", 2)]
        public BigInteger Amount { get; set; }
    }
}