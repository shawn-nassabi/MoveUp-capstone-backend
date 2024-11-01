using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using health_app_backend.DTOs;
using health_app_backend.Models;
using health_app_backend.Repositories;

namespace health_app_backend.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILocationRepository _locationRepository;

    public UserService(IUserRepository userRepository, ILocationRepository locationRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _locationRepository = locationRepository;
        _mapper = mapper;
    }
    
    // Get all users
    public async Task<IEnumerable<UserResponseDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        // Map the list of users to UserResponseDto
        return _mapper.Map<IEnumerable<UserResponseDto>>(users);
    }
    
    // Get user by Id
    public async Task<UserResponseDto> GetUserAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        return user != null ? _mapper.Map<UserResponseDto>(user) : null;
    }
    
    // Get user by username
    public async Task<UserResponseDto> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user != null ? _mapper.Map<UserResponseDto>(user) : null;
    }
    
    // Add new user
    public async Task<string> AddUserAsync(UserCreateDto userDto)
    {
        var user = _mapper.Map<User>(userDto); // Maps UserCreateDto to User
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        
        Console.WriteLine("Location id is: " + user.LocationId);
        var location = await _locationRepository.GetByIdAsync(user.LocationId);
        if (location == null)
        {
            throw new Exception("Invalid location id provided");
        }
        
        user.Location = location;
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user.Id.ToString();
    }
    
    // Update existing user
    public async Task<bool> UpdateUserAsync(string userId, UserUpdateDto updatedUser)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null)
        {
            return false; // User not found
        }

        // Update fields
        user.Age = updatedUser.Age;
        user.Gender = updatedUser.Gender;
        user.LocationId = updatedUser.LocationId;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
        return true;
    }
}