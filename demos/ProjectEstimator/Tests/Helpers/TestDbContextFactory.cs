using Microsoft.EntityFrameworkCore;
using ProjectEstimator.Data;
using ProjectEstimator.Models;

namespace ProjectEstimator.Tests.Helpers;

public static class TestDbContextFactory
{
    public static ApplicationDbContext CreateInMemoryContext(string databaseName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName + Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        return context;
    }

    public static ApplicationDbContext CreateContextWithSeedData(string databaseName = "SeedTestDb")
    {
        var context = CreateInMemoryContext(databaseName);
        SeedTestData(context);
        return context;
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        // Seed Projects
        var project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Description = "A test project for unit testing"
        };
        context.Projects.Add(project);

        // Seed Users
        var users = new List<User>
        {
            new User { Id = 1, Name = "John Developer", Role = UserRole.Developer, Email = "john@test.com", Department = "Engineering" },
            new User { Id = 2, Name = "Jane Designer", Role = UserRole.Designer, Email = "jane@test.com", Department = "Design" },
            new User { Id = 3, Name = "Bob Manager", Role = UserRole.ProjectManager, Email = "bob@test.com", Department = "Management" }
        };
        context.Users.AddRange(users);

        // Seed Tasks
        var tasks = new List<ProjectTask>
        {
            new ProjectTask { Id = 1, Name = "Task 1", ProjectId = 1, OptimisticHours = 2, MostLikelyHours = 4, PessimisticHours = 8 },
            new ProjectTask { Id = 2, Name = "Task 2", ProjectId = 1, OptimisticHours = 1, MostLikelyHours = 2, PessimisticHours = 4 }
        };
        context.Tasks.AddRange(tasks);

        context.SaveChanges();
    }
}