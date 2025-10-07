using ProjectEstimator.Models;

namespace ProjectEstimator.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Check if data already exists
        if (context.Projects.Any())
        {
            return; // Database has been seeded
        }
        
        // Create sample users first
        var users = new[]
        {
            new User
            {
                Name = "Alice Johnson",
                Role = UserRole.ProjectManager,
                Email = "alice.johnson@company.com",
                Department = "Engineering",
                IsActive = true
            },
            new User
            {
                Name = "Bob Smith",
                Role = UserRole.Developer,
                Email = "bob.smith@company.com",
                Department = "Engineering",
                IsActive = true
            },
            new User
            {
                Name = "Carol Williams",
                Role = UserRole.Designer,
                Email = "carol.williams@company.com",
                Department = "Design",
                IsActive = true
            },
            new User
            {
                Name = "David Brown",
                Role = UserRole.QualityAssurance,
                Email = "david.brown@company.com",
                Department = "Quality",
                IsActive = true
            },
            new User
            {
                Name = "Eva Davis",
                Role = UserRole.Analyst,
                Email = "eva.davis@company.com",
                Department = "Business",
                IsActive = true
            }
        };
        
        context.Users.AddRange(users);
        context.SaveChanges();
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
        
        // Create task assignments
        var savedUsers = context.Users.ToList();
        var taskAssignments = new[]
        {
            // Requirements Analysis - Eva (Analyst) as leader, Alice (PM) supporting
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "Eva Davis").Id,
                TaskId = savedTasks[0].Id,
                IsLeader = true,
                AllocationPercentage = 80,
                AssignedDate = DateTime.Today
            },
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "Alice Johnson").Id,
                TaskId = savedTasks[0].Id,
                IsLeader = false,
                AllocationPercentage = 50,
                AssignedDate = DateTime.Today
            },
            
            // System Design - Bob (Developer) as leader
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "Bob Smith").Id,
                TaskId = savedTasks[1].Id,
                IsLeader = true,
                AllocationPercentage = 100,
                AssignedDate = DateTime.Today.AddDays(3)
            },
            
            // Database Development - Bob (Developer) as leader
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "Bob Smith").Id,
                TaskId = savedTasks[2].Id,
                IsLeader = true,
                AllocationPercentage = 100,
                AssignedDate = DateTime.Today.AddDays(7)
            },
            
            // UI Development - Carol (Designer) as leader, Bob supporting
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "Carol Williams").Id,
                TaskId = savedTasks[3].Id,
                IsLeader = true,
                AllocationPercentage = 100,
                AssignedDate = DateTime.Today.AddDays(7)
            },
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "Bob Smith").Id,
                TaskId = savedTasks[3].Id,
                IsLeader = false,
                AllocationPercentage = 50,
                AssignedDate = DateTime.Today.AddDays(10)
            },
            
            // Integration Testing - David (QA) as leader, Bob supporting
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "David Brown").Id,
                TaskId = savedTasks[4].Id,
                IsLeader = true,
                AllocationPercentage = 100,
                AssignedDate = DateTime.Today.AddDays(15)
            },
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "Bob Smith").Id,
                TaskId = savedTasks[4].Id,
                IsLeader = false,
                AllocationPercentage = 30,
                AssignedDate = DateTime.Today.AddDays(15)
            },
            
            // User Acceptance Testing - David (QA) as leader, Alice (PM) and Eva (Analyst) supporting
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "David Brown").Id,
                TaskId = savedTasks[5].Id,
                IsLeader = true,
                AllocationPercentage = 80,
                AssignedDate = DateTime.Today.AddDays(18)
            },
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "Alice Johnson").Id,
                TaskId = savedTasks[5].Id,
                IsLeader = false,
                AllocationPercentage = 40,
                AssignedDate = DateTime.Today.AddDays(18)
            },
            new TaskAssignment
            {
                UserId = savedUsers.First(u => u.Name == "Eva Davis").Id,
                TaskId = savedTasks[5].Id,
                IsLeader = false,
                AllocationPercentage = 60,
                AssignedDate = DateTime.Today.AddDays(18)
            }
        };
        
        context.TaskAssignments.AddRange(taskAssignments);
        
        context.SaveChanges();
    }
}
