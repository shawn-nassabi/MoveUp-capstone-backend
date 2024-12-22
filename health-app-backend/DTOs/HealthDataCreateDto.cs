namespace health_app_backend.DTOs;

public class HealthDataCreateDto
{
    public Guid UserId { get; set; }            // User's ID
    public int DatatypeId { get; set; }         // ID of the data type (e.g., steps, heart rate)
    public float DataValue { get; set; }        // The health data value
    public DateTime RecordedAt { get; set; }    // Date and time of data recording
    public int TimeZoneOffset { get; set; } // Time zone offset in minutes
}