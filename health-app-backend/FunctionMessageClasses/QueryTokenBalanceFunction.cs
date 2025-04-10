using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace health_app_backend.FunctionMessageClasses
{
    [Function("balanceOf", "uint256")]
    public class QueryTokenBalanceFunction : FunctionMessage
    {
        [Parameter("address", "account", 1)]
        public string Account { get; set; }
    }
}