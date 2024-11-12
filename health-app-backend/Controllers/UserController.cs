using health_app_backend.DTOs;
using health_app_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    // Get all users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }
    
    // Get a user using their userID
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserResponseDto>> GetUserById(string userId)
    {
        var user = await _userService.GetUserAsync(userId);
        if (user == null)
        {
            return NotFound(); // Returns 404 if the user isn't found
        }
        return Ok(user);
    }
    
    // Get a user using their username
    [HttpGet("username/{username}")]
    public async Task<ActionResult<UserResponseDto>> GetUserByUsername(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
    
    // Create a new user
    [HttpPost]
    public async Task<ActionResult<string>> CreateUser(UserCreateDto newUser)
    {
        try
        {
            var userId = await _userService.AddUserAsync(newUser);
            return CreatedAtAction(nameof(GetUserById), new { userId = userId }, userId);
        }
        catch (Exception ex)
        {
            // Log the exception (if a logger is available)
            return StatusCode(500, "An error occurred while creating the user.");
        }
    }
    
    // Update an existing user
    [HttpPut("{userId}")]
    public async Task<ActionResult> UpdateUser(string userId, UserUpdateDto updatedUser)
    {
        var updateSuccess = await _userService.UpdateUserAsync(userId, updatedUser);
        if (!updateSuccess)
        {
            return NotFound("User not found.");
        }
        return NoContent(); // 204 No Content if the update was successful
    }
}