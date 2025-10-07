using Microsoft.EntityFrameworkCore;
using ProjectEstimator.Models;

namespace ProjectEstimator.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectTask> Tasks { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<TaskAssignment> TaskAssignments { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Project entity
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.StartDate).IsRequired();
        });
        
        // Configure ProjectTask entity
        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.OptimisticHours).IsRequired();
            entity.Property(e => e.MostLikelyHours).IsRequired();
            entity.Property(e => e.PessimisticHours).IsRequired();
            
            // Foreign key to Project
            entity.HasOne(e => e.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure many-to-many relationship for task dependencies
        modelBuilder.Entity<ProjectTask>()
            .HasMany(t => t.Dependencies)
            .WithMany(t => t.Dependents)
            .UsingEntity<Dictionary<string, object>>(
                "TaskDependencies",
                j => j.HasOne<ProjectTask>().WithMany().HasForeignKey("DependencyId"),
                j => j.HasOne<ProjectTask>().WithMany().HasForeignKey("TaskId")
            );
        
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Department).HasMaxLength(50);
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            
            // Index for performance
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.IsActive);
        });
        
        // Configure TaskAssignment entity
        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IsLeader).IsRequired();
            entity.Property(e => e.AllocationPercentage).IsRequired();
            entity.Property(e => e.AssignedDate).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            // Foreign key relationships
            entity.HasOne(e => e.User)
                  .WithMany(u => u.TaskAssignments)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Task)
                  .WithMany(t => t.TaskAssignments)
                  .HasForeignKey(e => e.TaskId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes for performance
            entity.HasIndex(e => new { e.UserId, e.TaskId });
            entity.HasIndex(e => e.IsLeader);
            entity.HasIndex(e => e.AssignedDate);
            
            // Business rule: Only one leader per task per active assignment
            entity.HasIndex(e => new { e.TaskId, e.IsLeader })
                  .HasFilter("IsLeader = 1 AND UnassignedDate IS NULL")
                  .IsUnique();
        });
    }
}
