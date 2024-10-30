using System.ComponentModel.DataAnnotations;

namespace health_app_backend.Models;

public class UserBenchmarkRecord
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid DemographicBenchmarkId { get; set; }
    public DemographicBenchmark DemographicBenchmark { get; set; }
    public float DataValue { get; set; }
}