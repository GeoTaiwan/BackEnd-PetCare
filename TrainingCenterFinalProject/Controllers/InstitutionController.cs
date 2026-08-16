
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        var institutions = await _context.Institutions
            .Include(i=>i.InstitutionStatus)
            .Include(i => i.InstitutionType)
            .ToListAsync();
        return View(institutions);
    }

    // GET: INSTITUTIONS/Create
    public IActionResult Create()
    {
        //將type、status改為下拉選單
        PopulateInstitutionTypesAndStatuses();
        
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
        ModelState.Remove("InstitutionID"); // 移除 InstitutionID 的驗證，因為我們會自動生成它
        institution.InstitutionID = GenerateInstitutionID();
        if (ModelState.IsValid)
        {
            _context.Add(institution);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //將type、status改為下拉選單
        PopulateInstitutionTypesAndStatuses();
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
        //將type、status改為下拉選單
        PopulateInstitutionTypesAndStatuses();
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
        //將type、status改為下拉選單
        PopulateInstitutionTypesAndStatuses();
        return View(institution);
    }

    // POST: INSTITUTIONS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var appointments = await _context.Appointments.Where(a => a.InstitutionID == id).ToListAsync();
        if (appointments != null)
        {
            _context.Appointments.RemoveRange(appointments);
        }
        var authorizations = await _context.AuthorizationRecords.Where(a => a.InstitutionID == id).ToListAsync();
        if (authorizations != null)
        {
            _context.AuthorizationRecords.RemoveRange(authorizations);
        }
        var employments = await _context.Employments.Where(e => e.InstitutionID == id).ToListAsync();
        if (employments != null)
        {
            _context.Employments.RemoveRange(employments);
        }
        var taskAssignments = await _context.TaskSchedules.Where(t => t.InstitutionID == id).ToListAsync();
        if (taskAssignments != null)
        {
            _context.TaskSchedules.RemoveRange(taskAssignments);
        }
        await _context.SaveChangesAsync();
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

    private void PopulateInstitutionTypesAndStatuses()
    {
        ViewBag.InstitutionTypes = new SelectList(_context.InstitutionTypes, "TypeId", "Name");
        ViewBag.InstitutionStatuses = new SelectList(_context.InstitutionStatuses, "StatusId", "StatusName");
    }

}
