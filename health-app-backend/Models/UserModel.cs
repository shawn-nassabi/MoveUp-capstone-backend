using System;
using System.ComponentModel.DataAnnotations;

namespace health_app_backend.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int Age { get; set; }
    public string Gender { get; set; }
    
    public int LocationId { get; set; }
    public Location Location { get; set; }
}