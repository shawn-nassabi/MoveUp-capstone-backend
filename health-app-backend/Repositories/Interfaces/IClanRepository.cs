using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IClanRepository : IRepository<Clan>
{
    Task<Clan> GetClanWithMembersAsync(Guid clanId); // Retrieve a clan with all its members
    Task<IEnumerable<Clan>> GetClansByLocationAsync(string location); // Retrieve clans by location
    Task<bool> DeleteClanAsync(Guid clanId); // Delete clan based on clan Id
}