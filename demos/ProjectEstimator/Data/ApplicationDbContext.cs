using Microsoft.EntityFrameworkCore;
using ProjectEstimator.Models;

namespace ProjectEstimator.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectTask> Tasks { get; set; } = null!;
    
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
    }
}
