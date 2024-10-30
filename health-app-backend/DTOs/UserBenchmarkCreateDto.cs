namespace health_app_backend.DTOs;

public class UserBenchmarkCreateDto
{
    public Guid UserId { get; set; }
    public float DataValue { get; set; }
    public int DataTypeId { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public string TimeFrame { get; set; } // "week" or "month"
    public string Gender { get; set; }
    public int LocationId { get; set; }
}