using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IPointsRewardHistoryRepository : IRepository<PointsRewardHistory>
{
    IUnitOfWork UnitOfWork { get; }
    Task<IEnumerable<PointsRewardHistory>> GetByWalletAddressAsync(string walletAddress);

}