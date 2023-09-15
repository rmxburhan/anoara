namespace WebApi.dto.auth;
public record AuthenticationResponse(
    string Token,
    DateTime ExpiredAt
);