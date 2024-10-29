using System.ComponentModel.DataAnnotations;

namespace health_app_backend.Models;

public class Location
{
    [Key]
    public int Id { get; set; }
    public string CityName { get; set; }
    public string CountryName { get; set; }
}