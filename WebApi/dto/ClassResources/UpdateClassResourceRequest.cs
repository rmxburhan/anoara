namespace WebApi.dto.ClassResources;

public record UpdateClassResoureceRequest(
    string? Title,
    string? Message,
    IFormFile? AttachedFile
);