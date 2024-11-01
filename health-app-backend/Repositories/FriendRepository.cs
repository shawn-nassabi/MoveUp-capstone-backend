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
}