namespace WebApi.Models;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Banner { get; set; } = string.Empty;
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; }
    public virtual ICollection<Comment> Comments { get; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}