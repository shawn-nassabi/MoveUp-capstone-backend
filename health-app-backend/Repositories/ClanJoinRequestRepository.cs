using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class ClanJoinRequestRepository : Repository<ClanJoinRequest>, IClanJoinRequestRepository
{
    private readonly AppDbContext _context;

    public ClanJoinRequestRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClanJoinRequest>> GetPendingRequestsForClanAsync(Guid clanId)
    {
        return await _context.ClanJoinRequests
            .Where(r => r.ClanId == clanId && r.IsPending)
            .Include(r => r.User) // Include user details for each join request
            .Include(r => r.Clan)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClanJoinRequest>> GetPendingRequestsForUserAsync(Guid userId)
    {
        return await _context.ClanJoinRequests
            .Where(r => r.UserId == userId && r.IsPending)
            .Include(r => r.Clan) // Include clan details for each join request
            .ToListAsync();
    }

    public async Task<bool> ApproveJoinRequestAsync(Guid requestId)
    {
        var request = await _context.ClanJoinRequests.FindAsync(requestId);
        if (request == null || !request.IsPending)
        {
            return false; // Request not found or already processed
        }

        request.IsPending = false;
        request.IsApproved = true;

        var clanMember = new ClanMember
        {
            Id = Guid.NewGuid(),
            ClanId = request.ClanId,
            UserId = request.UserId,
            Role = "Member",
            JoinedAt = DateTime.UtcNow
        };

        await _context.ClanMembers.AddAsync(clanMember);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectJoinRequestAsync(Guid requestId)
    {
        var request = await _context.ClanJoinRequests.FindAsync(requestId);
        if (request != null && request.IsPending)
        {
            _context.ClanJoinRequests.Remove(request); // Remove rejected request
            await _context.SaveChangesAsync();
            return true;
        }
        return false; // The request doesn't exist or it isn't pending
    }
}