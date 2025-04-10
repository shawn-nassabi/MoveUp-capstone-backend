using System.Numerics;

namespace health_app_backend.Services;

public interface IBlockchainService
{
    Task SubmitDailyDataAsync(string userAddress, int dataTypes, bool hasCondition);
    Task<BigInteger> GetUserPointsAsync(string userAddress);
    Task SubmitDataHashAsync(Guid userId, string dataHash);
    Task ConvertPointsToTokensAsync(Guid userId);
    Task<BigInteger> GetTokensMintedEventAsync(string userAddress);
    Task MintTokensAsync(string userAddress, BigInteger amount);
    Task<BigInteger> GetPointsPerTokenAsync();
    Task<decimal> GetUserTokenBalanceAsync(Guid userId);
}