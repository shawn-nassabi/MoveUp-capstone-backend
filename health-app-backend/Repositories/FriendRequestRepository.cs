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

    public async Task<String> SendFriendRequestAsync(Guid senderId, Guid receiverId)
    {
        // Check if a friend request already exists (pending or accepted)
        var existingRequest = await _context.FriendRequests
            .FirstOrDefaultAsync(fr =>
                (fr.SenderId == senderId && fr.ReceiverId == receiverId) ||
                (fr.SenderId == receiverId && fr.ReceiverId == senderId));
        
        if (existingRequest != null)
        {
            if (existingRequest.IsPending)
            {
                return "A friend request is already pending.";
            }
            if (existingRequest.IsAccepted)
            {
                return "You are already friends.";
            }
        }
        
        // Create the friend request if all checks pass
        try
        {
            var request = new FriendRequest
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                SentAt = DateTime.UtcNow,
                IsPending = true,
                IsAccepted = false
            };

            await _context.FriendRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            return "Friend request sent successfully.";
        }
        catch (Exception ex)
        {
            // Log exception if you have a logging mechanism
            return $"An error occurred while sending the friend request: {ex.Message}";
        }
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

    public async Task DeclineFriendRequestAsync(Guid requestId)
    {
        var request = await _context.FriendRequests.FindAsync(requestId);
        if (request != null && request.IsPending)
        {
            _context.FriendRequests.Remove(request); // Remove the friend request from the database
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<FriendRequest>> GetPendingRequestsAsync(Guid userId)
    {
        return await _context.FriendRequests
            .Where(r => r.ReceiverId == userId && r.IsPending)
            .Include(r => r.Sender)
            .ToListAsync();
    }
}