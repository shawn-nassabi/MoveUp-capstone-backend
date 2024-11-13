namespace health_app_backend.DTOs;

public class ClanDetailsDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public IEnumerable<ClanMemberDto> Members { get; set; }
}