using System.Text.Json.Serialization;

namespace WebApi.Models;

public class AnnouncementComment
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public Guid AnnouncementId { get; set; }
    [JsonIgnore]
    public Announcement Announcement { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}