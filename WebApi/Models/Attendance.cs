using System.Text.Json.Serialization;

namespace WebApi.Models;

public class Attendance
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public Guid ClassId { get; set; }
    [JsonIgnore]
    public Class Class { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}