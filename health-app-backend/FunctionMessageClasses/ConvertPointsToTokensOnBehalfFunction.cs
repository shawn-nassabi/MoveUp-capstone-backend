using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace health_app_backend.FunctionMessageClasses
{
    [Function("convertPointsToTokensOnBehalf")]
    public class ConvertPointsToTokensOnBehalfFunction : FunctionMessage
    {
        [Parameter("address", "user", 1)]
        public string User { get; set; }
    }
}