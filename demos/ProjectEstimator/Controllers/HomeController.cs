using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectEstimator.Data;
using ProjectEstimator.Models;
using ProjectEstimator.Services;

namespace ProjectEstimator.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEstimationService _estimationService;
    private readonly IMermaidService _mermaidService;

    public HomeController(ApplicationDbContext context, IEstimationService estimationService, IMermaidService mermaidService)
    {
        _context = context;
        _estimationService = estimationService;
        _mermaidService = mermaidService;
    }

    public async Task<IActionResult> Index()
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .ThenInclude(t => t.Dependencies)
            .FirstOrDefaultAsync();
            
        if (project == null)
        {
            return View("NoProject");
        }
        
        // Calculate critical path
        var criticalTasks = _estimationService.CalculateCriticalPath(project.Tasks.ToList());
        
        // Get project summary
        var summary = _estimationService.CalculateProjectSummary(project);
        
        ViewBag.ProjectSummary = summary;
        ViewBag.GanttChart = _mermaidService.GenerateGanttChart(project);
        ViewBag.DependencyChart = _mermaidService.GenerateTaskDependencyChart(project.Tasks.ToList());
        
        return View(project);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
