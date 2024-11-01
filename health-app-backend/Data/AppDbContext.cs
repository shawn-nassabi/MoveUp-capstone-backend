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
    public DbSet<UserBenchmarkRecordModel> UserBenchmarkRecords { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }
    public DbSet<Friend> Friends { get; set; }
    
    // Need the following to resolve migration issues with the new friends and friendrequests models
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Friend>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Friend>()
            .HasOne(f => f.FriendUser)
            .WithMany()
            .HasForeignKey(f => f.FriendId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

