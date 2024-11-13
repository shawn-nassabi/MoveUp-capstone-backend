using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class ClanMemberRepository : Repository<ClanMember>, IClanMemberRepository
{
    private readonly AppDbContext context;
    public ClanMemberRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClanMember>> GetAllByClanId(Guid clanId)
    {
        // Get all members for a specific clan, including each member's User details
        return await _context.ClanMembers
            .Where(cm => cm.ClanId == clanId)
            .Include(cm => cm.User)
            .ToListAsync();
    }

    public async Task<ClanMember> GetByIdAsync(Guid memberId)
    {
        // Get specific clan member by ID, including related clan details
        return await _context.ClanMembers
            .Include(cm => cm.Clan)
            .Include(cm => cm.User)
            .FirstOrDefaultAsync(cm => cm.Id == memberId);
    }

    public async Task<ClanMember> GetByUserIdAsync(Guid userId)
    {
        return await _context.ClanMembers
            .Include(cm => cm.Clan)
            .Include(cm => cm.User)
            .FirstOrDefaultAsync(cm => cm.UserId == userId);
    }
    
    public async Task<bool> RemoveMemberFromClanAsync(Guid clanId, Guid userId)
    {
        var member = await _context.ClanMembers
            .FirstOrDefaultAsync(cm => cm.ClanId == clanId && cm.UserId == userId);
    
        if (member == null)
        {
            return false; // Member not found in the specified clan
        }

        _context.ClanMembers.Remove(member);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<ClanMember> GetMemberDetailsAsync(Guid clanId, Guid userId)
    {
        return await _context.ClanMembers
            .Include(cm => cm.User) // Include user details
            .Include(cm => cm.Clan) // Include clan details
            .FirstOrDefaultAsync(cm => cm.ClanId == clanId && cm.UserId == userId);
    }
}