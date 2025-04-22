using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface ITokenRewardHistoryRepository : IRepository<TokenRewardHistory>
{
    IUnitOfWork UnitOfWork { get; }
    Task<IEnumerable<TokenRewardHistory>> GetByWalletAddressAsync(string walletAddress);
}