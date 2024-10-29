using System.Threading.Tasks;
using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<Location> GetByIdAsync(int id);
}