namespace WebApi.dto.Student;

public record LoginRequest(
    string Username,
    string Password
);