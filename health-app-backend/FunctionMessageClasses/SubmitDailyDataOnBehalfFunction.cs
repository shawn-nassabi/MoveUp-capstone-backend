namespace health_app_backend.FunctionMessageClasses;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

// Function message class
// This is the recommended pattern when making blockchain calls
/*
 * When you call a smart contract function, you need to pass specific parameters (e.g., a wallet address, a token amount, etc.).
 * When the function returns data, you need to decode it into a format your application can understand.
 * Function Message Classes handle this encoding and decoding process in a clean and maintainable way.
 */
[Function("submitDailyDataOnBehalf")]
public class SubmitDailyDataOnBehalfFunction : FunctionMessage
{
    [Parameter("address", "user", 1)]
    public string User { get; set; }

    [Parameter("uint256", "dataTypes", 2)]
    public BigInteger DataTypes { get; set; }

    [Parameter("bool", "hasCondition", 3)]
    public bool HasCondition { get; set; }
}