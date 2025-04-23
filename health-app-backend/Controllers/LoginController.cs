using health_app_backend.DTOs;
using health_app_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public LoginController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return Ok(new
        {
            userId = user.Id,
            username = user.Username,
            walletAddress = user.WalletAddress,
            age = user.Age,
            gender = user.Gender,
            locationId = user.LocationId
        });
    }
}