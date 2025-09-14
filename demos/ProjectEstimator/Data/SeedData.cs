using ProjectEstimator.Models;

namespace ProjectEstimator.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Create sample project
        var project = new Project
        {
            Name = "Sample Software Project",
            Description = "A demonstration project showing three-point estimation and task dependencies",
            StartDate = DateTime.Today
        };
        
        context.Projects.Add(project);
        context.SaveChanges();
        
        // Create sample tasks with three-point estimates
        var tasks = new[]
        {
            new ProjectTask
            {
                Name = "Requirements Analysis",
                Description = "Gather and document project requirements",
                OptimisticHours = 16,
                MostLikelyHours = 24,
                PessimisticHours = 40,
                Priority = 1,
                ProjectId = project.Id,
                StartDate = DateTime.Today
            },
            new ProjectTask
            {
                Name = "System Design",
                Description = "Create system architecture and design documents",
                OptimisticHours = 20,
                MostLikelyHours = 32,
                PessimisticHours = 50,
                Priority = 2,
                ProjectId = project.Id,
                StartDate = DateTime.Today.AddDays(3)
            },
            new ProjectTask
            {
                Name = "Database Development",
                Description = "Design and implement database schema",
                OptimisticHours = 12,
                MostLikelyHours = 20,
                PessimisticHours = 32,
                Priority = 3,
                ProjectId = project.Id,
                StartDate = DateTime.Today.AddDays(7)
            },
            new ProjectTask
            {
                Name = "User Interface Development",
                Description = "Develop user interface components",
                OptimisticHours = 24,
                MostLikelyHours = 40,
                PessimisticHours = 60,
                Priority = 3,
                ProjectId = project.Id,
                StartDate = DateTime.Today.AddDays(7)
            },
            new ProjectTask
            {
                Name = "Integration Testing",
                Description = "Test integration between components",
                OptimisticHours = 8,
                MostLikelyHours = 16,
                PessimisticHours = 28,
                Priority = 4,
                ProjectId = project.Id,
                StartDate = DateTime.Today.AddDays(15)
            },
            new ProjectTask
            {
                Name = "User Acceptance Testing",
                Description = "Conduct testing with end users",
                OptimisticHours = 12,
                MostLikelyHours = 20,
                PessimisticHours = 35,
                Priority = 5,
                ProjectId = project.Id,
                StartDate = DateTime.Today.AddDays(18)
            }
        };
        
        foreach (var task in tasks)
        {
            task.EndDate = task.StartDate?.AddHours(task.ExpectedHours);
            context.Tasks.Add(task);
        }
        
        context.SaveChanges();
        
        // Add dependencies after tasks are saved (so we have IDs)
        var savedTasks = context.Tasks.Where(t => t.ProjectId == project.Id).ToList();
        
        // Requirements -> System Design
        savedTasks[1].Dependencies.Add(savedTasks[0]);
        
        // System Design -> Database Development
        savedTasks[2].Dependencies.Add(savedTasks[1]);
        
        // System Design -> UI Development
        savedTasks[3].Dependencies.Add(savedTasks[1]);
        
        // Database + UI -> Integration Testing
        savedTasks[4].Dependencies.Add(savedTasks[2]);
        savedTasks[4].Dependencies.Add(savedTasks[3]);
        
        // Integration Testing -> User Acceptance Testing
        savedTasks[5].Dependencies.Add(savedTasks[4]);
        
        context.SaveChanges();
    }
}
