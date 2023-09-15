namespace WebApi.dto.ClassResources;

public record AddClassResoureceRequest(
    string Title,
    string Message,
    IFormFile AttachedFile,
    Guid ClassId
);