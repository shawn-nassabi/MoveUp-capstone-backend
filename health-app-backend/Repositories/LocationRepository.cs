using health_app_backend.Models;

namespace health_app_backend.Repositories;

public class LocationRepository : Repository<Location>
{
    public LocationRepository(AppDbContext context) : base(context) { }
}