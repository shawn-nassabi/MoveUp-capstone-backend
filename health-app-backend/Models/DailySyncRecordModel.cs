using System.ComponentModel.DataAnnotations;

namespace health_app_backend.Models;

// This model is used to keep track of whether data for a given date has already been synced with the blockchain
public class DailySyncRecord
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public DateTime Date { get; set; }  // Represents the day of the data that was synced (local date)

    public bool SyncedOnChain { get; set; }

    public DateTime SyncedAt { get; set; }  // When the data was synced to the blockchain
}