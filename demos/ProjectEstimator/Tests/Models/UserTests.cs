using FluentAssertions;
using NUnit.Framework;
using ProjectEstimator.Models;
using ProjectEstimator.Tests.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ProjectEstimator.Tests.Models;

[TestFixture]
public class UserTests
{
    [Test]
    public void User_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Name.Should().Be(string.Empty);
        user.Role.Should().Be(UserRole.Developer);
        user.Email.Should().Be(string.Empty);
        user.Department.Should().Be(string.Empty);
        user.IsActive.Should().BeTrue();
        user.TaskAssignments.Should().NotBeNull().And.BeEmpty();
        user.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Test]
    public void User_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var user = TestDataBuilder.CreateValidUser(
            name: "John Doe",
            role: UserRole.ProjectManager,
            email: "john.doe@company.com",
            department: "Engineering");

        // Assert
        user.Name.Should().Be("John Doe");
        user.Role.Should().Be(UserRole.ProjectManager);
        user.Email.Should().Be("john.doe@company.com");
        user.Department.Should().Be("Engineering");
        user.IsActive.Should().BeTrue();
    }

    [TestCase("")]
    [TestCase(null)]
    public void User_WithInvalidName_FailsValidation(string invalidName)
    {
        // Arrange
        var user = TestDataBuilder.CreateValidUser(name: invalidName);
        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains("Name"));
    }

    [Test]
    public void User_WithNameTooLong_FailsValidation()
    {
        // Arrange
        var longName = new string('A', 101); // Exceeds 100 character limit
        var user = TestDataBuilder.CreateValidUser(name: longName);
        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains("Name"));
    }

    [Test]
    public void User_WithEmailTooLong_FailsValidation()
    {
        // Arrange
        var longEmail = new string('a', 96) + "@test.com"; // Exceeds 100 character limit
        var user = TestDataBuilder.CreateValidUser(email: longEmail);
        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains("Email"));
    }

    [Test]
    public void User_WithDepartmentTooLong_FailsValidation()
    {
        // Arrange
        var longDepartment = new string('D', 51); // Exceeds 50 character limit
        var user = TestDataBuilder.CreateValidUser(department: longDepartment);
        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains("Department"));
    }

    [Test]
    public void User_AssignedTasks_ReturnsTasksFromAssignments()
    {
        // Arrange
        var user = TestDataBuilder.CreateValidUser();
        var task1 = TestDataBuilder.CreateValidProjectTask("Task 1");
        var task2 = TestDataBuilder.CreateValidProjectTask("Task 2");
        
        user.TaskAssignments = new List<TaskAssignment>
        {
            new TaskAssignment { Task = task1 },
            new TaskAssignment { Task = task2 }
        };

        // Act
        var assignedTasks = user.AssignedTasks;

        // Assert
        assignedTasks.Should().HaveCount(2);
        assignedTasks.Should().Contain(task1);
        assignedTasks.Should().Contain(task2);
    }

    [Test]
    public void User_LeadingTasks_ReturnsOnlyLeaderAssignments()
    {
        // Arrange
        var user = TestDataBuilder.CreateValidUser();
        var task1 = TestDataBuilder.CreateValidProjectTask("Task 1");
        var task2 = TestDataBuilder.CreateValidProjectTask("Task 2");
        var task3 = TestDataBuilder.CreateValidProjectTask("Task 3");
        
        user.TaskAssignments = new List<TaskAssignment>
        {
            new TaskAssignment { Task = task1, IsLeader = true },
            new TaskAssignment { Task = task2, IsLeader = false },
            new TaskAssignment { Task = task3, IsLeader = true }
        };

        // Act
        var leadingTasks = user.LeadingTasks;

        // Assert
        leadingTasks.Should().HaveCount(2);
        leadingTasks.Should().Contain(task1);
        leadingTasks.Should().Contain(task3);
        leadingTasks.Should().NotContain(task2);
    }

    [Test]
    public void User_AllUserRoles_AreValid()
    {
        // Arrange
        var expectedRoles = new[]
        {
            UserRole.Developer,
            UserRole.Designer,
            UserRole.ProjectManager,
            UserRole.QualityAssurance,
            UserRole.DevOps,
            UserRole.Analyst,
            UserRole.Architect
        };

        // Act & Assert
        foreach (var role in expectedRoles)
        {
            var user = TestDataBuilder.CreateValidUser(role: role);
            user.Role.Should().Be(role);
        }
    }

    [Test]
    public void User_WithValidData_PassesValidation()
    {
        // Arrange
        var user = TestDataBuilder.CreateValidUser();
        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }
}