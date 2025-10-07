using FluentAssertions;
using NUnit.Framework;
using ProjectEstimator.Models;
using ProjectEstimator.Tests.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ProjectEstimator.Tests.Models;

[TestFixture]
public class TaskAssignmentTests
{
    [Test]
    public void TaskAssignment_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var assignment = new TaskAssignment();

        // Assert
        assignment.IsLeader.Should().BeFalse();
        assignment.AllocationPercentage.Should().Be(100);
        assignment.AssignedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.UnassignedDate.Should().BeNull();
        assignment.Notes.Should().Be(string.Empty);
    }

    [Test]
    public void TaskAssignment_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var assignment = TestDataBuilder.CreateValidTaskAssignment(
            userId: 1,
            taskId: 2,
            isLeader: true,
            allocationPercentage: 75,
            notes: "Lead developer for this task");

        // Assert
        assignment.UserId.Should().Be(1);
        assignment.TaskId.Should().Be(2);
        assignment.IsLeader.Should().BeTrue();
        assignment.AllocationPercentage.Should().Be(75);
        assignment.Notes.Should().Be("Lead developer for this task");
    }

    [TestCase(-1)]
    [TestCase(101)]
    [TestCase(150)]
    public void TaskAssignment_WithInvalidAllocationPercentage_FailsValidation(int invalidPercentage)
    {
        // Arrange
        var assignment = TestDataBuilder.CreateValidTaskAssignment(allocationPercentage: invalidPercentage);
        var validationContext = new ValidationContext(assignment);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(assignment, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(vr => 
            vr.MemberNames.Contains("AllocationPercentage") && 
            vr.ErrorMessage.Contains("between 0 and 100"));
    }

    [TestCase(0)]
    [TestCase(50)]
    [TestCase(100)]
    public void TaskAssignment_WithValidAllocationPercentage_PassesValidation(int validPercentage)
    {
        // Arrange
        var assignment = TestDataBuilder.CreateValidTaskAssignment(allocationPercentage: validPercentage);
        var validationContext = new ValidationContext(assignment);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(assignment, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [Test]
    public void TaskAssignment_WithNotesTooLong_FailsValidation()
    {
        // Arrange
        var longNotes = new string('N', 501); // Exceeds 500 character limit
        var assignment = TestDataBuilder.CreateValidTaskAssignment(notes: longNotes);
        var validationContext = new ValidationContext(assignment);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(assignment, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains("Notes"));
    }

    [Test]
    public void TaskAssignment_IsActive_WhenUnassignedDateIsNull_ReturnsTrue()
    {
        // Arrange
        var assignment = TestDataBuilder.CreateValidTaskAssignment();
        assignment.UnassignedDate = null;

        // Act & Assert
        assignment.IsActive.Should().BeTrue();
    }

    [Test]
    public void TaskAssignment_IsActive_WhenUnassignedDateIsSet_ReturnsFalse()
    {
        // Arrange
        var assignment = TestDataBuilder.CreateValidTaskAssignment();
        assignment.UnassignedDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        assignment.IsActive.Should().BeFalse();
    }

    [Test]
    public void TaskAssignment_AssignmentDuration_WhenActive_CalculatesFromAssignedDateToNow()
    {
        // Arrange
        var assignedDate = DateTime.UtcNow.AddDays(-5);
        var assignment = TestDataBuilder.CreateValidTaskAssignment();
        assignment.AssignedDate = assignedDate;
        assignment.UnassignedDate = null;

        // Act
        var duration = assignment.AssignmentDuration;

        // Assert
        duration.Should().BeCloseTo(TimeSpan.FromDays(5), TimeSpan.FromMinutes(1));
    }

    [Test]
    public void TaskAssignment_AssignmentDuration_WhenInactive_CalculatesFromAssignedDateToUnassignedDate()
    {
        // Arrange
        var assignedDate = DateTime.UtcNow.AddDays(-10);
        var unassignedDate = DateTime.UtcNow.AddDays(-3);
        var assignment = TestDataBuilder.CreateValidTaskAssignment();
        assignment.AssignedDate = assignedDate;
        assignment.UnassignedDate = unassignedDate;

        // Act
        var duration = assignment.AssignmentDuration;

        // Assert
        duration.Should().BeCloseTo(TimeSpan.FromDays(7), TimeSpan.FromSeconds(1));
    }

    [Test]
    public void TaskAssignment_WithFutureAssignedDate_CalculatesDurationCorrectly()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);
        var assignment = TestDataBuilder.CreateValidTaskAssignment();
        assignment.AssignedDate = futureDate;
        assignment.UnassignedDate = null;

        // Act
        var duration = assignment.AssignmentDuration;

        // Assert
        duration.Should().BeCloseTo(TimeSpan.FromDays(-1), TimeSpan.FromMinutes(1));
    }

    [Test]
    public void TaskAssignment_LeaderAssignment_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var assignment = TestDataBuilder.CreateValidTaskAssignment(
            isLeader: true,
            allocationPercentage: 100,
            notes: "Project lead");

        // Assert
        assignment.IsLeader.Should().BeTrue();
        assignment.AllocationPercentage.Should().Be(100);
        assignment.Notes.Should().Be("Project lead");
    }

    [Test]
    public void TaskAssignment_PartialAllocation_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var assignment = TestDataBuilder.CreateValidTaskAssignment(
            isLeader: false,
            allocationPercentage: 25,
            notes: "Part-time contributor");

        // Assert
        assignment.IsLeader.Should().BeFalse();
        assignment.AllocationPercentage.Should().Be(25);
        assignment.Notes.Should().Be("Part-time contributor");
    }

    [Test]
    public void TaskAssignment_WithValidData_PassesValidation()
    {
        // Arrange
        var assignment = TestDataBuilder.CreateValidTaskAssignment();
        var validationContext = new ValidationContext(assignment);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(assignment, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [Test]
    public void TaskAssignment_UnassignTask_SetsUnassignedDate()
    {
        // Arrange
        var assignment = TestDataBuilder.CreateValidTaskAssignment();
        var unassignDate = DateTime.UtcNow.AddHours(-2);

        // Act
        assignment.UnassignedDate = unassignDate;

        // Assert
        assignment.UnassignedDate.Should().Be(unassignDate);
        assignment.IsActive.Should().BeFalse();
        assignment.AssignmentDuration.Should().BeCloseTo(
            unassignDate - assignment.AssignedDate, 
            TimeSpan.FromSeconds(1));
    }
}