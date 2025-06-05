using System;
using System.ComponentModel.DataAnnotations;

namespace health_app_backend.Models;

public class HealthData
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public int DatatypeId { get; set; }
    public DataType Datatype { get; set; }
    
    public float DataValue { get; set; }
    public DateTime RecordedAt { get; set; } // Stored in UTC
    public int?    TimeZoneOffset { get; set; }  // new: user's local timezone offset from utc in minutes
}