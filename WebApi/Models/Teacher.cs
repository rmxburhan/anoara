using System.Text.Json.Serialization;

namespace WebApi.Models;

public class Teacher
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    [JsonIgnore]
    public string Password { get; set; }
    public string ProfilePicture { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}