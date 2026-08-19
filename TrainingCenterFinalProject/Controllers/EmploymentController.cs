
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

public class EmploymentController : Controller
{
    private readonly PetCareDbContext _context;

    public EmploymentController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: EMPLOYMENTS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Employments.ToListAsync());
    }


    // GET: EMPLOYMENTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: EMPLOYMENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employment employment)
    {
        if (ModelState.IsValid)
        {
            _context.Add(employment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(employment);
    }

    // GET: EMPLOYMENTS/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employment = await _context.Employments.FindAsync(id);
        if (employment == null)
        {
            return NotFound();
        }
        return View(employment);
    }

    // POST: EMPLOYMENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? id, Employment employment)
    {
        if (id != employment.UserID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(employment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmploymentExists(employment.UserID))
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
        return View(employment);
    }

    // GET: EMPLOYMENTS/Delete/5
    public async Task<IActionResult> Delete(string? userid)
    {
        if (userid == null)
        {
            return NotFound();
        }

        var employment = await _context.Employments
            .FirstOrDefaultAsync(m => m.UserID == userid);
        if (employment == null)
        {
            return NotFound();
        }

        return View(employment);
    }

    // POST: EMPLOYMENTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? userid)
    {
        var employment = await _context.Employments.FindAsync(userid);
        if (employment != null)
        {
            _context.Employments.Remove(employment);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EmploymentExists(string? userid)
    {
        return _context.Employments.Any(e => e.UserID == userid);
    }
}
