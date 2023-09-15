namespace WebApi.dto.Attendance;

public record AddAttendanceRequest(
    Guid ClassId,
    Guid[] Students
);