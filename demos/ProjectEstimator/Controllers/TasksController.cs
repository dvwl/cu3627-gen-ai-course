using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectEstimator.Data;
using ProjectEstimator.Models;
using ProjectEstimator.Services;

namespace ProjectEstimator.Controllers;

public class TasksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEstimationService _estimationService;

    public TasksController(ApplicationDbContext context, IEstimationService estimationService)
    {
        _context = context;
        _estimationService = estimationService;
    }

    // GET: Tasks
    public async Task<IActionResult> Index()
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Dependencies)
            .ToListAsync();
        return View(tasks);
    }

    // GET: Tasks/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var task = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Dependencies)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (task == null) return NotFound();

        // Get tasks that depend on this task
        var dependentTasks = await _context.Tasks
            .Where(t => t.Dependencies.Any(d => d.Id == id))
            .ToListAsync();

        ViewBag.DependentTasks = dependentTasks;

        // Calculate estimation details
        var estimation = _estimationService.CalculateThreePointEstimate(
            task.OptimisticHours, task.MostLikelyHours, task.PessimisticHours);
        ViewBag.Estimation = estimation;

        return View(task);
    }

    // GET: Tasks/Create
    public async Task<IActionResult> Create()
    {
        var project = await _context.Projects.FirstOrDefaultAsync();
        if (project == null)
        {
            return RedirectToAction("CreateProject");
        }

        var availableTasks = await _context.Tasks
            .Where(t => t.ProjectId == project.Id)
            .ToListAsync();
            
        ViewBag.Project = project;
        ViewBag.AvailableTasks = availableTasks;
        
        return View(new ProjectTask { ProjectId = project.Id });
    }

    // POST: Tasks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,OptimisticHours,MostLikelyHours,PessimisticHours,StartDate,Priority,Status,ProjectId")] ProjectTask task, int[] dependencyIds)
    {
        if (ModelState.IsValid)
        {
            // Calculate end date based on expected hours
            if (task.StartDate.HasValue)
            {
                task.EndDate = task.StartDate.Value.AddHours(task.ExpectedHours);
            }

            _context.Add(task);
            await _context.SaveChangesAsync();
            
            // Add dependencies
            if (dependencyIds != null && dependencyIds.Length > 0)
            {
                var dependencies = await _context.Tasks
                    .Where(t => dependencyIds.Contains(t.Id))
                    .ToListAsync();
                    
                task.Dependencies = dependencies;
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        var project = await _context.Projects.FirstOrDefaultAsync();
        var availableTasks = await _context.Tasks
            .Where(t => t.ProjectId == project!.Id)
            .ToListAsync();
            
        ViewBag.Project = project;
        ViewBag.AvailableTasks = availableTasks;
        
        return View(task);
    }

    // GET: Tasks/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var task = await _context.Tasks
            .Include(t => t.Dependencies)
            .FirstOrDefaultAsync(t => t.Id == id);
            
        if (task == null) return NotFound();

        var availableTasks = await _context.Tasks
            .Where(t => t.ProjectId == task.ProjectId && t.Id != task.Id)
            .ToListAsync();
            
        ViewBag.AvailableTasks = availableTasks;
        ViewBag.CurrentDependencies = task.Dependencies.Select(d => d.Id).ToArray();
        
        return View(task);
    }

    // POST: Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,OptimisticHours,MostLikelyHours,PessimisticHours,StartDate,Priority,Status,ProjectId,PercentComplete")] ProjectTask task, int[] dependencyIds)
    {
        if (id != task.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                // Calculate end date based on expected hours
                if (task.StartDate.HasValue)
                {
                    task.EndDate = task.StartDate.Value.AddHours(task.ExpectedHours);
                }

                _context.Update(task);
                await _context.SaveChangesAsync();
                
                // Update dependencies
                var existingTask = await _context.Tasks
                    .Include(t => t.Dependencies)
                    .FirstOrDefaultAsync(t => t.Id == id);
                    
                if (existingTask != null)
                {
                    existingTask.Dependencies.Clear();
                    
                    if (dependencyIds != null && dependencyIds.Length > 0)
                    {
                        var dependencies = await _context.Tasks
                            .Where(t => dependencyIds.Contains(t.Id))
                            .ToListAsync();
                            
                        foreach (var dep in dependencies)
                        {
                            existingTask.Dependencies.Add(dep);
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        
        var availableTasks = await _context.Tasks
            .Where(t => t.ProjectId == task.ProjectId && t.Id != task.Id)
            .ToListAsync();
            
        ViewBag.AvailableTasks = availableTasks;
        
        return View(task);
    }

    // GET: Tasks/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var task = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Dependencies)
            .FirstOrDefaultAsync(m => m.Id == id);
            
        if (task == null) return NotFound();

        // Get tasks that depend on this task
        var dependentTasks = await _context.Tasks
            .Where(t => t.Dependencies.Any(d => d.Id == id))
            .ToListAsync();

        ViewBag.DependentTasks = dependentTasks;

        return View(task);
    }

    // POST: Tasks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TaskExists(int id)
    {
        return _context.Tasks.Any(e => e.Id == id);
    }
    
    public IActionResult CreateProject()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProject([Bind("Name,Description,StartDate")] Project project)
    {
        if (ModelState.IsValid)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Create));
        }
        return View(project);
    }
}
