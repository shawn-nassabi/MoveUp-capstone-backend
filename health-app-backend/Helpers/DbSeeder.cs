namespace health_app_backend.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using health_app_backend.Models;


public static class DbSeeder
{
    public static async Task SeedUsersAsync(AppDbContext context)
    {
        // Only seed if no users exist yet
        // if (context.Users.Any()) return;

        // 1) Read wallets from JSON
        // Ensure the JSON file is copied to the output directory and referenced by its filename
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "wallets-test-users.json");
        var walletsJson = File.ReadAllText(jsonPath);
        var wallets     = JsonConvert.DeserializeObject<List<WalletInfo>>(walletsJson);
        Console.WriteLine($"Wallets loaded: {wallets.Count}");

        var genders    = new[] { "Male", "Female", "Other" };
        var locationIds = new[] { 1, 2 };
        var rng        = new Random();

        for (int i = 0; i < wallets.Count; i++)
        {
            var w = wallets[i];
            var user = new User
            {
                
                Id = Guid.NewGuid(),
                Username = $"user{i+1:D2}",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword($"Password{i+1:D2}!"),
                Age = rng.Next(18, 26),
                Gender = genders[i % genders.Length],
                LocationId = locationIds[i % locationIds.Length],
                WalletAddress = w.Address
                // leave PrivateKey safe in your JSON file for backend gas payments if needed
            };
            Console.WriteLine($"Creating user: {user.Username}, {user.WalletAddress}");
            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
        Console.WriteLine("✅ Seeded 20 test users with real wallets");
    }

    private class WalletInfo
    {
        public string Address { get; set; }
        public string PrivateKey { get; set; }
    }
}