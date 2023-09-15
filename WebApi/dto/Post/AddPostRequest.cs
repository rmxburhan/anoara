namespace WebApi.dto.Post;

public class AddPostRequest
{

    public string Title { get; set; }
    public string Content { get; set; }
    public IFormFile? Banner { get; set; }
}