using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectEstimator.Models;

public class TaskAssignment
{
    public int Id { get; set; }
    
    // Foreign keys
    public int UserId { get; set; }
    public int TaskId { get; set; }
    
    // Assignment properties
    public bool IsLeader { get; set; } = false;
    
    [Range(0, 100, ErrorMessage = "Allocation percentage must be between 0 and 100")]
    public int AllocationPercentage { get; set; } = 100;
    
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UnassignedDate { get; set; }
    
    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ProjectTask Task { get; set; } = null!;
    
    // Computed properties
    [NotMapped]
    public bool IsActive => !UnassignedDate.HasValue;
    [NotMapped]
    public TimeSpan AssignmentDuration => (UnassignedDate ?? DateTime.UtcNow) - AssignedDate;
}