using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class FriendRequestRepository : Repository<FriendRequest>, IFriendRequestRepository
{
    private readonly AppDbContext _context;

    public FriendRequestRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task SendFriendRequestAsync(Guid senderId, Guid receiverId)
    {
        var request = new FriendRequest
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId
        };
        await _context.FriendRequests.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task AcceptFriendRequestAsync(Guid requestId)
    {
        var request = await _context.FriendRequests.FindAsync(requestId);
        if (request != null && request.IsPending)
        {
            request.IsPending = false;
            request.IsAccepted = true;

            // Add mutual friendship entries in Friends table
            var friendship1 = new Friend { UserId = request.SenderId, FriendId = request.ReceiverId };
            var friendship2 = new Friend { UserId = request.ReceiverId, FriendId = request.SenderId };

            await _context.Friends.AddRangeAsync(friendship1, friendship2);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<FriendRequest>> GetPendingRequestsAsync(Guid userId)
    {
        return await _context.FriendRequests
            .Where(r => r.ReceiverId == userId && r.IsPending)
            .ToListAsync();
    }
}