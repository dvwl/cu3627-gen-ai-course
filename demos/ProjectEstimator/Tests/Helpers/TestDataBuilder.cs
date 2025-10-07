using ProjectEstimator.Models;

namespace ProjectEstimator.Tests.Helpers;

public static class TestDataBuilder
{
    public static User CreateValidUser(
        string name = "Test User",
        UserRole role = UserRole.Developer,
        string email = "test@example.com",
        string department = "Test Department")
    {
        return new User
        {
            Name = name,
            Role = role,
            Email = email,
            Department = department,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };
    }

    public static TaskAssignment CreateValidTaskAssignment(
        int userId = 1,
        int taskId = 1,
        bool isLeader = false,
        int allocationPercentage = 100,
        string notes = "Test assignment")
    {
        return new TaskAssignment
        {
            UserId = userId,
            TaskId = taskId,
            IsLeader = isLeader,
            AllocationPercentage = allocationPercentage,
            AssignedDate = DateTime.UtcNow,
            Notes = notes
        };
    }

    public static ProjectTask CreateValidProjectTask(
        string name = "Test Task",
        int projectId = 1,
        double optimistic = 2.0,
        double mostLikely = 4.0,
        double pessimistic = 8.0)
    {
        return new ProjectTask
        {
            Name = name,
            ProjectId = projectId,
            OptimisticHours = optimistic,
            MostLikelyHours = mostLikely,
            PessimisticHours = pessimistic
        };
    }

    public static Project CreateValidProject(
        string name = "Test Project",
        string description = "Test Description")
    {
        return new Project
        {
            Name = name,
            Description = description
        };
    }
}