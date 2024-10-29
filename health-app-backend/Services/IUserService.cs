using health_app_backend.DTOs;
using health_app_backend.Models;

namespace health_app_backend.Services;

public interface IUserService
{
    public Task<IEnumerable<UserResponseDto>> GetUsersAsync();
    public Task<UserResponseDto> GetUserAsync(string userId);
    public Task<UserResponseDto> GetUserByUsernameAsync(string username);
    public Task<string> AddUserAsync(UserCreateDto user);
}