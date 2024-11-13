using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class ClanRepository : Repository<Clan>, IClanRepository
{
    private readonly AppDbContext _context;
    public ClanRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<Clan> GetClanWithMembersAsync(Guid clanId)
    {
        return await _context.Clans
            .Include(c => c.Members)
            .ThenInclude(m => m.User) // Include user details for each member
            .FirstOrDefaultAsync(c => c.Id == clanId);
    }

    public async Task<IEnumerable<Clan>> GetClansByLocationAsync(string location)
    {
        return await _context.Clans
            .Where(c => c.Location == location)
            .ToListAsync();
    }
}