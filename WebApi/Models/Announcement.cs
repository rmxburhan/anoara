namespace WebApi.Models;

public class Announcement
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; }
    public virtual ICollection<AnnouncementComment> AnnouncementComments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}