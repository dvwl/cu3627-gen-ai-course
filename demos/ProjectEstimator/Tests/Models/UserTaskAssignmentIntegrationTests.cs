using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectEstimator.Data;
using ProjectEstimator.Models;
using ProjectEstimator.Tests.Helpers;

namespace ProjectEstimator.Tests.Models;

[TestFixture]
public class UserTaskAssignmentIntegrationTests
{
    private ApplicationDbContext _context;

    [SetUp]
    public void SetUp()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task User_CanBeSavedToDatabase_WithValidData()
    {
        // Arrange
        var user = TestDataBuilder.CreateValidUser(
            name: "Integration Test User",
            email: "integration@test.com");

        // Act
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == "integration@test.com");
        
        savedUser.Should().NotBeNull();
        savedUser.Name.Should().Be("Integration Test User");
        savedUser.Email.Should().Be("integration@test.com");
    }

    [Test]
    public async Task TaskAssignment_CanBeSavedToDatabase_WithValidData()
    {
        // Arrange
        await SeedDatabaseWithUserAndTask();
        
        var assignment = new TaskAssignment
        {
            UserId = 1,
            TaskId = 1,
            IsLeader = true,
            AllocationPercentage = 80,
            Notes = "Integration test assignment"
        };

        // Act
        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Assert
        var savedAssignment = await _context.TaskAssignments
            .FirstOrDefaultAsync(ta => ta.Notes == "Integration test assignment");
        
        savedAssignment.Should().NotBeNull();
        savedAssignment.UserId.Should().Be(1);
        savedAssignment.TaskId.Should().Be(1);
        savedAssignment.IsLeader.Should().BeTrue();
        savedAssignment.AllocationPercentage.Should().Be(80);
    }

    [Test]
    public async Task User_WithTaskAssignments_LoadsNavigationProperties()
    {
        // Arrange
        await SeedDatabaseWithUserAndTask();
        
        var assignment = new TaskAssignment
        {
            UserId = 1,
            TaskId = 1,
            IsLeader = true,
            AllocationPercentage = 100
        };
        
        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Act
        var userWithAssignments = await _context.Users
            .Include(u => u.TaskAssignments)
            .ThenInclude(ta => ta.Task)
            .FirstOrDefaultAsync(u => u.Id == 1);

        // Assert
        userWithAssignments.Should().NotBeNull();
        userWithAssignments.TaskAssignments.Should().HaveCount(1);
        userWithAssignments.TaskAssignments.First().Task.Should().NotBeNull();
        userWithAssignments.AssignedTasks.Should().HaveCount(1);
        userWithAssignments.LeadingTasks.Should().HaveCount(1);
    }

    [Test]
    public async Task TaskAssignment_WithUserAndTask_LoadsNavigationProperties()
    {
        // Arrange
        await SeedDatabaseWithUserAndTask();
        
        var assignment = new TaskAssignment
        {
            UserId = 1,
            TaskId = 1,
            AllocationPercentage = 50
        };
        
        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Act
        var assignmentWithNav = await _context.TaskAssignments
            .Include(ta => ta.User)
            .Include(ta => ta.Task)
            .FirstOrDefaultAsync();

        // Assert
        assignmentWithNav.Should().NotBeNull();
        assignmentWithNav.User.Should().NotBeNull();
        assignmentWithNav.User.Name.Should().Be("Test User");
        assignmentWithNav.Task.Should().NotBeNull();
        assignmentWithNav.Task.Name.Should().Be("Test Task");
    }

    [Test]
    public async Task User_CanHaveMultipleTaskAssignments()
    {
        // Arrange
        await SeedDatabaseWithUserAndMultipleTasks();
        
        var assignments = new List<TaskAssignment>
        {
            new TaskAssignment { UserId = 1, TaskId = 1, IsLeader = true, AllocationPercentage = 50 },
            new TaskAssignment { UserId = 1, TaskId = 2, IsLeader = false, AllocationPercentage = 30 }
        };
        
        _context.TaskAssignments.AddRange(assignments);
        await _context.SaveChangesAsync();

        // Act
        var userWithMultipleAssignments = await _context.Users
            .Include(u => u.TaskAssignments)
            .ThenInclude(ta => ta.Task)
            .FirstOrDefaultAsync(u => u.Id == 1);

        // Assert
        userWithMultipleAssignments.Should().NotBeNull();
        userWithMultipleAssignments.TaskAssignments.Should().HaveCount(2);
        userWithMultipleAssignments.AssignedTasks.Should().HaveCount(2);
        userWithMultipleAssignments.LeadingTasks.Should().HaveCount(1);
    }

    [Test]
    public async Task TaskAssignment_DeletingUser_RemovesAssignments()
    {
        // Arrange
        await SeedDatabaseWithUserAndTask();
        
        var assignment = new TaskAssignment
        {
            UserId = 1,
            TaskId = 1,
            AllocationPercentage = 100
        };
        
        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Act
        var user = await _context.Users.FindAsync(1);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // Assert
        var remainingAssignments = await _context.TaskAssignments.ToListAsync();
        remainingAssignments.Should().BeEmpty();
    }

    [Test]
    public async Task User_AssignedTasks_ComputedProperty_WorksCorrectly()
    {
        // Arrange
        await SeedDatabaseWithUserAndMultipleTasks();
        
        var assignments = new List<TaskAssignment>
        {
            new TaskAssignment { UserId = 1, TaskId = 1, IsLeader = true },
            new TaskAssignment { UserId = 1, TaskId = 2, IsLeader = false }
        };
        
        _context.TaskAssignments.AddRange(assignments);
        await _context.SaveChangesAsync();

        // Act
        var user = await _context.Users
            .Include(u => u.TaskAssignments)
            .ThenInclude(ta => ta.Task)
            .FirstOrDefaultAsync(u => u.Id == 1);

        // Assert
        user.AssignedTasks.Should().HaveCount(2);
        user.AssignedTasks.Should().Contain(t => t.Name == "Test Task 1");
        user.AssignedTasks.Should().Contain(t => t.Name == "Test Task 2");
    }

    private async Task SeedDatabaseWithUserAndTask()
    {
        var project = new Project { Name = "Test Project", Description = "Test" };
        var user = TestDataBuilder.CreateValidUser();
        var task = TestDataBuilder.CreateValidProjectTask();
        
        _context.Projects.Add(project);
        _context.Users.Add(user);
        task.ProjectId = project.Id;
        _context.Tasks.Add(task);
        
        await _context.SaveChangesAsync();
    }

    private async Task SeedDatabaseWithUserAndMultipleTasks()
    {
        var project = new Project { Name = "Test Project", Description = "Test" };
        var user = TestDataBuilder.CreateValidUser();
        var task1 = TestDataBuilder.CreateValidProjectTask("Test Task 1");
        var task2 = TestDataBuilder.CreateValidProjectTask("Test Task 2");
        
        _context.Projects.Add(project);
        _context.Users.Add(user);
        task1.ProjectId = project.Id;
        task2.ProjectId = project.Id;
        _context.Tasks.AddRange(task1, task2);
        
        await _context.SaveChangesAsync();
    }
}