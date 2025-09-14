using System.ComponentModel.DataAnnotations;

namespace ProjectEstimator.Models;

public class Project
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; } = DateTime.Today;
    
    public DateTime? EndDate { get; set; }
    
    public List<ProjectTask> Tasks { get; set; } = new();
    
    // Calculated properties
    public double TotalOptimisticHours => Tasks.Sum(t => t.OptimisticHours);
    public double TotalMostLikelyHours => Tasks.Sum(t => t.MostLikelyHours);
    public double TotalPessimisticHours => Tasks.Sum(t => t.PessimisticHours);
    public double TotalExpectedHours => Tasks.Sum(t => t.ExpectedHours);
    public double ProjectStandardDeviation => Math.Sqrt(Tasks.Sum(t => Math.Pow(t.StandardDeviation, 2)));
    
    public DateTime CalculatedEndDate 
    { 
        get 
        {
            if (!Tasks.Any()) return StartDate;
            return Tasks.Where(t => t.EndDate.HasValue).DefaultIfEmpty().Max(t => t?.EndDate) ?? StartDate;
        } 
    }
}
