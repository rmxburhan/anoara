namespace WebApi.dto.Class;

public class UpdateDataClassRequest
{
    public string? Topic { get; set; }
    public Guid? TecherId { get; set; }
    public DateTime? Time { get; set; }
}