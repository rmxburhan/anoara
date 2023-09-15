namespace WebApi.dto.Milestone;

public class AddMilestoneRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}