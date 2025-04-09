namespace health_app_backend.Services;

public interface IBlockchainService
{
    Task SubmitDailyDataAsync(string userAddress, int dataTypes, bool hasCondition);
}