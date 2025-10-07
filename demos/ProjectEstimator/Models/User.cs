using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectEstimator.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public UserRole Role { get; set; } = UserRole.Developer;
    
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string Department { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public List<TaskAssignment> TaskAssignments { get; set; } = new();
    
    // Computed properties
    [NotMapped]
    public List<ProjectTask> AssignedTasks => TaskAssignments.Select(ta => ta.Task).ToList();
    [NotMapped]
    public List<ProjectTask> LeadingTasks => TaskAssignments.Where(ta => ta.IsLeader).Select(ta => ta.Task).ToList();
}

public enum UserRole
{
    Developer,
    Designer,
    ProjectManager,
    QualityAssurance,
    DevOps,
    Analyst,
    Architect
}