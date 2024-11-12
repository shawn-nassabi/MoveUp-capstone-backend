using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class FriendRepository : Repository<Friend>, IFriendRepository
{
    private readonly AppDbContext _context;

    public FriendRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetFriendsAsync(Guid userId)
    {
        return await _context.Friends
            .Where(f => f.UserId == userId)
            .Select(f => f.FriendUser) // Assuming FriendUser navigates to the User entity
            .ToListAsync();
    }

    public async Task<bool> DeleteFriendAsync(Guid userId, Guid friendId)
    {
        // Find both friendship records in the Friends table
        var friendship1 = await _context.Friends
            .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);

        var friendship2 = await _context.Friends
            .FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId);

        // If either friendship record exists, remove them
        if (friendship1 != null)
        {
            _context.Friends.Remove(friendship1);
        }

        if (friendship2 != null)
        {
            _context.Friends.Remove(friendship2);
        }

        // Save changes if either record was removed
        if (friendship1 != null || friendship2 != null)
        {
            await _context.SaveChangesAsync();
            return true; // Successfully deleted the friendship
        }

        return false; // No friendship record found, nothing to delete
    }
}