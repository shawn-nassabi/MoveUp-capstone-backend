namespace health_app_backend.DTOs;

public class ClanCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string LeaderId { get; set; }
}