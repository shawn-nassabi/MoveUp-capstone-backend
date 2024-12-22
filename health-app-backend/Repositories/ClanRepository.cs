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

    public async Task<bool> DeleteClanAsync(Guid clanId)
    {
        // Find the clan by its ID
        var clan = await _context.Clans.FindAsync(clanId);

        // If the clan does not exist, return false
        if (clan == null)
        {
            return false;
        }

        // Remove the clan from the DbContext
        _context.Clans.Remove(clan);

        // Save changes to the database
        await _context.SaveChangesAsync();

        // Return true to indicate successful deletion
        return true;
    }
}