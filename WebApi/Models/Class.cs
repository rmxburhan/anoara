namespace WebApi.Models;
public class Class
{
    public Guid Id { get; set; }
    public string Topic { get; set; }
    public Guid TeacherId { get; set; }
    public virtual Teacher Teacher { get; set; }
    public DateTime Time { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}