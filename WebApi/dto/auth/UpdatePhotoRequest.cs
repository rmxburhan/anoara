namespace WebApi.dto.auth;

public record UpdatePhotoRequest(
    IFormFile? ProfilePicture,
    IFormFile? BannerPicture
);