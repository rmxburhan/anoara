namespace WebApi.dto.Milestone;

public record AddMilestoneToStudent(
    Guid MilestoneId,
    Guid[] Students
);