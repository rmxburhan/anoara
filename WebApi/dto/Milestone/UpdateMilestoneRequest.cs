namespace WebApi.dto.Milestone;

public class UpdateMilestoneRequest
{
    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}