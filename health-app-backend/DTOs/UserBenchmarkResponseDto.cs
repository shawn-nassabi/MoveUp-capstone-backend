namespace health_app_backend.DTOs;

public class UserBenchmarkResponseDto
{
    public Guid Id { get; set; }
    public string AgeRange { get; set; }
    public string Gender { get; set; }
    public string TimeFrame { get; set; }
    public float UserDataValue { get; set; }
    public float AverageValue { get; set; }
    public float RecommendedValue { get; set; }
    public string LocationName { get; set; }
}