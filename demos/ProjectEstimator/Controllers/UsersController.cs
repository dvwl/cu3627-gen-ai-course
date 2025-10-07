using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectEstimator.Data;
using ProjectEstimator.Models;

namespace ProjectEstimator.Controllers;

public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Users
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .Include(u => u.TaskAssignments)
                .ThenInclude(ta => ta.Task)
            .OrderBy(u => u.Name)
            .ToListAsync();
        
        return View(users);
    }

    // GET: Users/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .Include(u => u.TaskAssignments)
                .ThenInclude(ta => ta.Task)
                    .ThenInclude(t => t.Project)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Role,Email,Department,IsActive")] User user)
    {
        if (ModelState.IsValid)
        {
            user.CreatedDate = DateTime.UtcNow;
            _context.Add(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"User '{user.Name}' has been created successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Role,Email,Department,IsActive,CreatedDate")] User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User '{user.Name}' has been updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
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
        return View(user);
    }

    // GET: Users/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .Include(u => u.TaskAssignments)
                .ThenInclude(ta => ta.Task)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _context.Users
            .Include(u => u.TaskAssignments)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user != null)
        {
            // Check if user has active task assignments
            var activeAssignments = user.TaskAssignments.Where(ta => ta.IsActive).Count();
            
            if (activeAssignments > 0)
            {
                TempData["ErrorMessage"] = $"Cannot delete user '{user.Name}' because they have {activeAssignments} active task assignment(s). Please unassign them from all tasks first.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"User '{user.Name}' has been deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Users/Assignments/5
    public async Task<IActionResult> Assignments(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .Include(u => u.TaskAssignments)
                .ThenInclude(ta => ta.Task)
                    .ThenInclude(t => t.Project)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}