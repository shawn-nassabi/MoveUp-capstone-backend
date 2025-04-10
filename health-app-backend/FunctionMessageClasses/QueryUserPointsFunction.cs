using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace health_app_backend.FunctionMessageClasses
{
    // This attribute indicates that this function queries the public variable "userPoints" in the smart contract,
    // and it returns a value of type "uint256" (mapped to BigInteger in C#).
    [Function("userPoints", "uint256")]
    public class QueryUserPointsFunction : FunctionMessage
    {
        // This parameter corresponds to the "user" address in the mapping.
        [Parameter("address", "user", 1)]
        public string User { get; set; }
    }
}