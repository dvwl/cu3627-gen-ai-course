using System.ComponentModel.DataAnnotations;

namespace ProjectEstimator.Models;

public class ProjectTask
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.1, 1000, ErrorMessage = "Optimistic hours must be between 0.1 and 1000")]
    public double OptimisticHours { get; set; } = 1;
    
    [Required]
    [Range(0.1, 1000, ErrorMessage = "Most likely hours must be between 0.1 and 1000")]
    public double MostLikelyHours { get; set; } = 2;
    
    [Required]
    [Range(0.1, 1000, ErrorMessage = "Pessimistic hours must be between 0.1 and 1000")]
    public double PessimisticHours { get; set; } = 4;
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public int Priority { get; set; } = 1;
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
    
    // Foreign key
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    
    // Dependencies - many-to-many relationship
    public List<ProjectTask> Dependencies { get; set; } = new();
    public List<ProjectTask> Dependents { get; set; } = new();
    
    // Calculated properties - PERT estimation
    public double ExpectedHours => (OptimisticHours + 4 * MostLikelyHours + PessimisticHours) / 6;
    public double StandardDeviation => (PessimisticHours - OptimisticHours) / 6;
    public double Variance => Math.Pow(StandardDeviation, 2);
    
    // Confidence intervals
    public double ConfidenceInterval68Lower => ExpectedHours - StandardDeviation;
    public double ConfidenceInterval68Upper => ExpectedHours + StandardDeviation;
    public double ConfidenceInterval95Lower => ExpectedHours - 2 * StandardDeviation;
    public double ConfidenceInterval95Upper => ExpectedHours + 2 * StandardDeviation;
    
    public bool IsOnCriticalPath { get; set; }
    public double PercentComplete { get; set; }
}

public enum TaskStatus
{
    NotStarted,
    InProgress,
    Completed,
    Blocked
}
