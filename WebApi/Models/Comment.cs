using System.Text.Json.Serialization;

namespace WebApi.Models;

public class Comment
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public Guid StudentId { get; set; }
    public virtual Student Student { get; set; }
    public Guid PostId { get; set; }
    [JsonIgnore]
    public virtual Post Post { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}