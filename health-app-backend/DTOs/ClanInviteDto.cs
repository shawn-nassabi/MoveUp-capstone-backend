namespace health_app_backend.DTOs;

public class ClanInviteDto
{
    public string Id { get; set; }
    public string ClanName { get; set; }
    public string UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}