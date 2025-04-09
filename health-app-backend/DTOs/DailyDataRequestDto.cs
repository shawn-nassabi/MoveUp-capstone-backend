namespace health_app_backend.DTOs;

public class DailyDataRequestDto
{
    public string UserAddress { get; set; }
    public int DataTypes { get; set; }
    public bool HasCondition  { get; set; }
}