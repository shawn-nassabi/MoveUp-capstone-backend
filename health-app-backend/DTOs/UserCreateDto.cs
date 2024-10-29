namespace health_app_backend.DTOs;

public class UserCreateDto
{
    public string Username { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public int LocationId { get; set; }
}