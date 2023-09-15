using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

public class ApiDataContext : DbContext
{
    public ApiDataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<ClassResources> ClassResources { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Milestone> Milestones { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<AnnouncementComment> AnnouncementComments { get; set; }
}