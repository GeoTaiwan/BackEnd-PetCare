
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using Microsoft.AspNetCore.Authorization;
[Authorize(Roles ="Admin")]
public class InstitutionController : Controller
{
    private readonly PetCareDbContext _context;

    public InstitutionController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: INSTITUTIONS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Institutions.ToListAsync());
    }

    // GET: INSTITUTIONS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: INSTITUTIONS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Institution institution)
    {
        if(institution == null)
        {
            return BadRequest();
        }
        institution.InstitutionID = GenerateInstitutionID();
        if (ModelState.IsValid)
        {
            _context.Add(institution);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(institution);
    }

    // GET: INSTITUTIONS/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var institution = await _context.Institutions.FindAsync(id);
        if (institution == null)
        {
            return NotFound();
        }
        return View(institution);
    }

    // POST: INSTITUTIONS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? id, Institution institution)
    {
        if (id != institution.InstitutionID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(institution);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstitutionExists(institution.InstitutionID))
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
        return View(institution);
    }

    // GET: INSTITUTIONS/Delete/5
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var institution = await _context.Institutions
            .FirstOrDefaultAsync(m => m.InstitutionID == id);
        if (institution == null)
        {
            return NotFound();
        }

        return View(institution);
    }

    // POST: INSTITUTIONS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        var institution = await _context.Institutions.FindAsync(id);
        if (institution != null)
        {
            _context.Institutions.Remove(institution);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool InstitutionExists(string? institutionid)
    {
        return _context.Institutions.Any(e => e.InstitutionID == institutionid);
    }

    private string GenerateInstitutionID()
    {
        //用I+5碼流水號的方式生成唯一的InstitutionID
        var lastInstitution = _context.Institutions.OrderByDescending(i => i.InstitutionID).FirstOrDefault();
        string newID = "I00001";
        if (lastInstitution == null) { 
            return newID;
        } else
        {
            int lastIDNumber = int.Parse(lastInstitution.InstitutionID.Substring(1));
            newID = "I" + (lastIDNumber + 1).ToString("D5");
        }
        return newID;
    }

}
