using ProjectEstimator.Models;
using System.Text;

namespace ProjectEstimator.Services;

public interface IMermaidService
{
    string GenerateGanttChart(Project project);
    string GenerateTaskDependencyChart(List<ProjectTask> tasks);
}

public class MermaidService : IMermaidService
{
    public string GenerateGanttChart(Project project)
    {
        var sb = new StringBuilder();
        sb.AppendLine("gantt");
        sb.AppendLine($"    title {CleanTaskName(project.Name)}");
        sb.AppendLine("    dateFormat YYYY-MM-DD");
        sb.AppendLine("    axisFormat %m/%d");
        sb.AppendLine();
        
        var sortedTasks = project.Tasks.OrderBy(t => t.Priority).ToList();
        
        if (!sortedTasks.Any())
        {
            sb.AppendLine("    section No Tasks");
            sb.AppendLine("    Create your first task :milestone, 2024-01-01, 0d");
            return sb.ToString();
        }
        
        // Group tasks by status for better organization
        var sections = new Dictionary<Models.TaskStatus, string>
        {
            { Models.TaskStatus.NotStarted, "Planned Tasks" },
            { Models.TaskStatus.InProgress, "In Progress" },
            { Models.TaskStatus.Completed, "Completed" },
            { Models.TaskStatus.Blocked, "Blocked" }
        };
        
        foreach (var section in sections)
        {
            var sectionTasks = sortedTasks.Where(t => t.Status == section.Key).ToList();
            if (sectionTasks.Any())
            {
                sb.AppendLine($"    section {section.Value}");
                
                foreach (var task in sectionTasks)
                {
                    var taskName = CleanTaskName(task.Name);
                    var startDate = DateTime.Today.ToString("yyyy-MM-dd");
                    var duration = Math.Max(1, (int)Math.Ceiling(task.ExpectedHours / 8)); // Convert hours to days
                    
                    var status = task.Status switch
                    {
                        Models.TaskStatus.Completed => "done",
                        Models.TaskStatus.InProgress => "active",
                        Models.TaskStatus.Blocked => "crit",
                        _ => ""
                    };
                    
                    if (!string.IsNullOrEmpty(status))
                    {
                        sb.AppendLine($"    {taskName} :{status}, {startDate}, {duration}d");
                    }
                    else
                    {
                        sb.AppendLine($"    {taskName} :{startDate}, {duration}d");
                    }
                }
            }
        }
        
        return sb.ToString();
    }
    
    public string GenerateTaskDependencyChart(List<ProjectTask> tasks)
    {
        if (!tasks.Any())
        {
            return "graph TD\n    A[No tasks available]\n    A --> B[Create your first task]";
        }
        
        var sb = new StringBuilder();
        sb.AppendLine("graph TD");
        
        // Add all tasks as nodes
        foreach (var task in tasks)
        {
            var taskName = CleanTaskName(task.Name);
            var statusIcon = task.Status switch
            {
                Models.TaskStatus.Completed => "✓ ",
                Models.TaskStatus.InProgress => "⏳ ",
                Models.TaskStatus.Blocked => "🚫 ",
                _ => ""
            };
            
            var nodeId = $"T{task.Id}";
            var nodeStyle = task.IsOnCriticalPath ? ":::critical" : "";
            
            sb.AppendLine($"    {nodeId}[\"{statusIcon}{taskName}<br/>{task.ExpectedHours:F1}h\"]{nodeStyle}");
        }
        
        sb.AppendLine();
        
        // Add dependency arrows
        foreach (var task in tasks)
        {
            foreach (var dependency in task.Dependencies)
            {
                sb.AppendLine($"    T{dependency.Id} --> T{task.Id}");
            }
        }
        
        // If no dependencies exist, show a simple flow
        if (!tasks.Any(t => t.Dependencies.Any()))
        {
            var orderedTasks = tasks.OrderBy(t => t.Priority).ToList();
            for (int i = 0; i < orderedTasks.Count - 1; i++)
            {
                sb.AppendLine($"    T{orderedTasks[i].Id} -.-> T{orderedTasks[i + 1].Id}");
            }
        }
        
        sb.AppendLine();
        sb.AppendLine("    classDef critical fill:#ff6b6b,stroke:#333,stroke-width:3px,color:#fff");
        
        return sb.ToString();
    }
    
    private string CleanTaskName(string name)
    {
        // Remove characters that might cause issues in Mermaid
        return name.Replace("\"", "'").Replace("\n", " ").Replace("\r", "");
    }
}
