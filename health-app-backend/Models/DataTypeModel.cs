using System.ComponentModel.DataAnnotations;

namespace health_app_backend.Models;

public class DataType
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
}