using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace WebApi.Models;

public class Student
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    [JsonIgnore]
    public string Password { get; set; }
    public string ProfilePicture { get; set; } = string.Empty;
    public string BannerPicture { get; set; } = string.Empty;
    public List<Milestone> Milestones { get; set; } = new();
    [JsonIgnore]
    public List<Attendance> Attendances { get; set; }
    [NotMapped]
    public int AttendanceCuont { get; set; }
    public int TestPassed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}