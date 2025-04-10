using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace health_app_backend.FunctionMessageClasses
{
    [Function("pointsPerToken", "uint256")]
    public class QueryPointsPerTokenFunction : FunctionMessage
    {
        // No parameters needed because it's a simple public variable getter
    }
}