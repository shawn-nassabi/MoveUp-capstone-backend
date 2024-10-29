namespace health_app_backend.DTOs;

public class HealthDataResponseDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }        // User's display name
    public string DataTypeName { get; set; }    // Name of the DataType
    public float DataValue { get; set; }        // Value of the health data
    public DateTime RecordedAt { get; set; }    // Date and time of data recording
}