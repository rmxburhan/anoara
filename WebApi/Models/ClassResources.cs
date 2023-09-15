using System.Net.Http.Headers;

namespace WebApi.Models;

public class ClassResources
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string FileLocation { get; set; }
    public Guid ClassId { get; set; }
    public Class Class { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}