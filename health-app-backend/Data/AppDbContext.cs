using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Define DbSet properties for each table
    public DbSet<User> Users { get; set; }
    public DbSet<HealthData> HealthData { get; set; }
    public DbSet<DemographicBenchmark> DemographicBenchmarks { get; set; }
    public DbSet<DataType> DataTypes { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<UserBenchmarkRecord> UserBenchmarkRecords { get; set; }
}

