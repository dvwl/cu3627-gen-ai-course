using ProjectEstimator.Models;

namespace ProjectEstimator.Services;

public interface IEstimationService
{
    EstimationResult CalculateThreePointEstimate(double optimistic, double mostLikely, double pessimistic);
    ProjectEstimationSummary CalculateProjectSummary(Project project);
    List<ProjectTask> CalculateCriticalPath(List<ProjectTask> tasks);
}

public class EstimationService : IEstimationService
{
    public EstimationResult CalculateThreePointEstimate(double optimistic, double mostLikely, double pessimistic)
    {
        var expected = (optimistic + 4 * mostLikely + pessimistic) / 6;
        var standardDeviation = (pessimistic - optimistic) / 6;
        var variance = Math.Pow(standardDeviation, 2);
        
        return new EstimationResult
        {
            Optimistic = optimistic,
            MostLikely = mostLikely,
            Pessimistic = pessimistic,
            Expected = expected,
            StandardDeviation = standardDeviation,
            Variance = variance,
            ConfidenceInterval68Lower = expected - standardDeviation,
            ConfidenceInterval68Upper = expected + standardDeviation,
            ConfidenceInterval95Lower = expected - 2 * standardDeviation,
            ConfidenceInterval95Upper = expected + 2 * standardDeviation
        };
    }
    
    public ProjectEstimationSummary CalculateProjectSummary(Project project)
    {
        var tasks = project.Tasks;
        
        return new ProjectEstimationSummary
        {
            TotalTasks = tasks.Count,
            TotalOptimisticHours = tasks.Sum(t => t.OptimisticHours),
            TotalMostLikelyHours = tasks.Sum(t => t.MostLikelyHours),
            TotalPessimisticHours = tasks.Sum(t => t.PessimisticHours),
            TotalExpectedHours = tasks.Sum(t => t.ExpectedHours),
            ProjectStandardDeviation = Math.Sqrt(tasks.Sum(t => Math.Pow(t.StandardDeviation, 2))),
            EarliestStartDate = tasks.Where(t => t.StartDate.HasValue).Min(t => t.StartDate),
            LatestEndDate = tasks.Where(t => t.EndDate.HasValue).Max(t => t.EndDate),
            CompletedTasks = tasks.Count(t => t.Status == Models.TaskStatus.Completed),
            InProgressTasks = tasks.Count(t => t.Status == Models.TaskStatus.InProgress),
            NotStartedTasks = tasks.Count(t => t.Status == Models.TaskStatus.NotStarted),
            BlockedTasks = tasks.Count(t => t.Status == Models.TaskStatus.Blocked)
        };
    }
    
    public List<ProjectTask> CalculateCriticalPath(List<ProjectTask> tasks)
    {
        // Simple critical path calculation
        // In a full implementation, this would use CPM algorithm
        
        var criticalTasks = new List<ProjectTask>();
        
        // For this demo, we'll identify tasks that have dependencies and are on the longest path
        var tasksWithDependencies = tasks.Where(t => t.Dependencies.Any()).ToList();
        
        // Mark tasks as critical based on their position in dependency chain
        foreach (var task in tasks)
        {
            task.IsOnCriticalPath = IsTaskOnCriticalPath(task, tasks);
            if (task.IsOnCriticalPath)
            {
                criticalTasks.Add(task);
            }
        }
        
        return criticalTasks;
    }
    
    private bool IsTaskOnCriticalPath(ProjectTask task, List<ProjectTask> allTasks)
    {
        // Simplified critical path logic
        // A task is on critical path if it has the longest chain of dependencies
        return GetDependencyChainLength(task, allTasks, new HashSet<int>()) >= 2;
    }
    
    private int GetDependencyChainLength(ProjectTask task, List<ProjectTask> allTasks, HashSet<int> visited)
    {
        if (visited.Contains(task.Id))
            return 0; // Avoid infinite loops
            
        visited.Add(task.Id);
        
        if (!task.Dependencies.Any())
            return 1;
            
        return 1 + task.Dependencies.Max(dep => GetDependencyChainLength(dep, allTasks, visited));
    }
}

public class EstimationResult
{
    public double Optimistic { get; set; }
    public double MostLikely { get; set; }
    public double Pessimistic { get; set; }
    public double Expected { get; set; }
    public double StandardDeviation { get; set; }
    public double Variance { get; set; }
    public double ConfidenceInterval68Lower { get; set; }
    public double ConfidenceInterval68Upper { get; set; }
    public double ConfidenceInterval95Lower { get; set; }
    public double ConfidenceInterval95Upper { get; set; }
}

public class ProjectEstimationSummary
{
    public int TotalTasks { get; set; }
    public double TotalOptimisticHours { get; set; }
    public double TotalMostLikelyHours { get; set; }
    public double TotalPessimisticHours { get; set; }
    public double TotalExpectedHours { get; set; }
    public double ProjectStandardDeviation { get; set; }
    public DateTime? EarliestStartDate { get; set; }
    public DateTime? LatestEndDate { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int NotStartedTasks { get; set; }
    public int BlockedTasks { get; set; }
}
