using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using health_app_backend.Models;

namespace health_app_backend.Repositories;

public class LocationRepository : Repository<Location>, ILocationRepository
{
    private readonly AppDbContext _context;
    public LocationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    // Override GetByIdAsync to accept an int ID
    public async Task<Location> GetByIdAsync(int id)
    {
        return await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);
    }
}