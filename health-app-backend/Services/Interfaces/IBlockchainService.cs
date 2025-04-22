using System.Numerics;
using health_app_backend.DTOs;
using health_app_backend.Models;

namespace health_app_backend.Services;

public interface IBlockchainService
{
    Task SubmitDailyDataAsync(string userAddress, int dataTypes, bool hasCondition);
    Task<BigInteger> GetUserPointsAsync(Guid userID);
    Task SubmitDataHashAsync(Guid userId, string dataHash);
    Task ConvertPointsToTokensAsync(Guid userId);
    Task<BigInteger> GetTokensMintedEventAsync(string userAddress);
    Task MintTokensAsync(string userAddress, BigInteger amount);
    Task<BigInteger> GetPointsPerTokenAsync();
    Task<decimal> GetUserTokenBalanceAsync(Guid userId);
    Task<IEnumerable<PointsRewardHistoryDto>> GetPointsRewardHistoryAsync(Guid userId);
    Task<IEnumerable<TokenRewardHistoryDto>> GetTokenRewardHistoryAsync(Guid userId);
    Task SyncPointsRewardHistoryFromChainAsync();
    Task SyncTokenRewardHistoryFromChainAsync();

}