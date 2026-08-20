
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Data;

public class LogPhotoController : Controller
{
    private readonly PetCareDbContext _context;

    public LogPhotoController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: LOGPHOTOS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.LogPhotos.ToListAsync());
    }

    // GET: LOGPHOTOS/Details/5
    public async Task<IActionResult> Details(int? photoid)
    {
        if (photoid == null)
        {
            return NotFound();
        }

        var logphoto = await _context.LogPhotos
            .FirstOrDefaultAsync(m => m.PhotoID == photoid);
        if (logphoto == null)
        {
            return NotFound();
        }

        return View(logphoto);
    }

    // GET: LOGPHOTOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: LOGPHOTOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PhotoID,LogID,ImageURL,CareGroomingLog")] LogPhoto logphoto)
    {
        if (ModelState.IsValid)
        {
            _context.Add(logphoto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(logphoto);
    }

    // GET: LOGPHOTOS/Edit/5
    public async Task<IActionResult> Edit(int? photoid)
    {
        if (photoid == null)
        {
            return NotFound();
        }

        var logphoto = await _context.LogPhotos.FindAsync(photoid);
        if (logphoto == null)
        {
            return NotFound();
        }
        return View(logphoto);
    }

    // POST: LOGPHOTOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? photoid, [Bind("PhotoID,LogID,ImageURL,CareGroomingLog")] LogPhoto logphoto)
    {
        if (photoid != logphoto.PhotoID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(logphoto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogPhotoExists(logphoto.PhotoID))
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
        return View(logphoto);
    }

    // GET: LOGPHOTOS/Delete/5
    public async Task<IActionResult> Delete(int? photoid)
    {
        if (photoid == null)
        {
            return NotFound();
        }

        var logphoto = await _context.LogPhotos
            .FirstOrDefaultAsync(m => m.PhotoID == photoid);
        if (logphoto == null)
        {
            return NotFound();
        }

        return View(logphoto);
    }

    // POST: LOGPHOTOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? photoid)
    {
        var logphoto = await _context.LogPhotos.FindAsync(photoid);
        if (logphoto != null)
        {
            _context.LogPhotos.Remove(logphoto);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LogPhotoExists(int? photoid)
    {
        return _context.LogPhotos.Any(e => e.PhotoID == photoid);
    }
}
