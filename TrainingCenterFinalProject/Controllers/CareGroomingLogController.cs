
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;

public class CareGroomingLogController : Controller
{
    private readonly PetCareDbContext _context;

    public CareGroomingLogController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: CAREGROOMINGLOGS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.CareGroomingLogs.ToListAsync());
    }

    // GET: CAREGROOMINGLOGS/Details/5
    public async Task<IActionResult> Details(int? logid)
    {
        if (logid == null)
        {
            return NotFound();
        }

        var caregroominglog = await _context.CareGroomingLogs
            .FirstOrDefaultAsync(m => m.LogID == logid);
        if (caregroominglog == null)
        {
            return NotFound();
        }

        return View(caregroominglog);
    }

    // GET: CAREGROOMINGLOGS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CAREGROOMINGLOGS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("LogID,PetID,StaffID,LogDate,ActivityDetails,DietNotes,Pet,Staff,LogPhotos")] CareGroomingLog caregroominglog)
    {
        if (ModelState.IsValid)
        {
            _context.Add(caregroominglog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(caregroominglog);
    }

    // GET: CAREGROOMINGLOGS/Edit/5
    public async Task<IActionResult> Edit(int? logid)
    {
        if (logid == null)
        {
            return NotFound();
        }

        var caregroominglog = await _context.CareGroomingLogs.FindAsync(logid);
        if (caregroominglog == null)
        {
            return NotFound();
        }
        return View(caregroominglog);
    }

    // POST: CAREGROOMINGLOGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? logid, [Bind("LogID,PetID,StaffID,LogDate,ActivityDetails,DietNotes,Pet,Staff,LogPhotos")] CareGroomingLog caregroominglog)
    {
        if (logid != caregroominglog.LogID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(caregroominglog);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CareGroomingLogExists(caregroominglog.LogID))
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
        return View(caregroominglog);
    }

    // GET: CAREGROOMINGLOGS/Delete/5
    public async Task<IActionResult> Delete(int? logid)
    {
        if (logid == null)
        {
            return NotFound();
        }

        var caregroominglog = await _context.CareGroomingLogs
            .FirstOrDefaultAsync(m => m.LogID == logid);
        if (caregroominglog == null)
        {
            return NotFound();
        }

        return View(caregroominglog);
    }

    // POST: CAREGROOMINGLOGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? logid)
    {
        var caregroominglog = await _context.CareGroomingLogs.FindAsync(logid);
        if (caregroominglog != null)
        {
            _context.CareGroomingLogs.Remove(caregroominglog);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CareGroomingLogExists(int? logid)
    {
        return _context.CareGroomingLogs.Any(e => e.LogID == logid);
    }
}
