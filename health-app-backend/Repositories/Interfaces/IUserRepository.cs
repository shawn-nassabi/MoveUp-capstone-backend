using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByUsernameAsync(string username);
}