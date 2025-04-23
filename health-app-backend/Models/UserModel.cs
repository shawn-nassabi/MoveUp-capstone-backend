using System;
using System.ComponentModel.DataAnnotations;

namespace health_app_backend.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int Age { get; set; } // Stay within 18-25
    public string Gender { get; set; }
    
    public int LocationId { get; set; } // Value between 1-2
    public Location Location { get; set; }
    public string WalletAddress { get; set; } // Ethereum/Polygon wallet address (e.g., 0xabc123...)
    public string PasswordHash { get; set; }
}