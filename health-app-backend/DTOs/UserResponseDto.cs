namespace health_app_backend.DTOs;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string LocationName { get; set; } // Location.CityName
}