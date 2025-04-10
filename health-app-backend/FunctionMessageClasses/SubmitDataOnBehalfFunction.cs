using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace health_app_backend.FunctionMessageClasses
{
    [Function("submitDataOnBehalf")]
    public class SubmitDataOnBehalfFunction : FunctionMessage
    {
        [Parameter("address", "user", 1)]
        public string User { get; set; }

        [Parameter("bytes32", "dataHash", 2)]
        public byte[] DataHash { get; set; }
    }
}