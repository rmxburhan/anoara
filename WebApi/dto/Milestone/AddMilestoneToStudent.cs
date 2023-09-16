namespace WebApi.dto.Milestone;

public record AddMilestoneToStudent(
    Guid[] Milestones,
    Guid StudentId
);