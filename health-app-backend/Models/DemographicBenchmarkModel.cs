using System;
using System.ComponentModel.DataAnnotations;

namespace health_app_backend.Models;

public class DemographicBenchmark
{
    [Key]
    public Guid Id { get; set; }
    public string AgeRange { get; set; }
    public string Gender { get; set; }
    
    public int LocationId { get; set; }
    public Location Location { get; set; }
    
    public int DataTypeId { get; set; }
    public DataType DataType { get; set; }
    
    public string Timefram { get; set; }
    public float AverageValue { get; set; }
    public float RecommendedValue { get; set; }
    public DateTime CreatedAt { get; set; }
}