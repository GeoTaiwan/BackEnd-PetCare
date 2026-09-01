
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Data;
using PetCare.Controllers;

public class AuthorizationRecordController : BaseController
{
    private readonly PetCareDbContext _context;

    public AuthorizationRecordController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: AUTHORIZATIONRECORDS
    public async Task<IActionResult> Index()    
    {
        
        //var pets = await _context.Pets.Where(p => p.OwnerID == LoginUserId).ToListAsync();
        var ars = await _context.AuthorizationRecords
            .Include(ar => ar.Pet)
            .Include(ar => ar.Institution)
            .Where(ar => ar.Pet.OwnerID == LoginUserId)
            .ToListAsync();
        return View(ars);
    }

    // GET: AUTHORIZATIONRECORDS/Details/5
    public async Task<IActionResult> Details(string? dataauthorizationnumber)
    {
        if (dataauthorizationnumber == null)
        {
            return NotFound();
        }

        var authorizationrecord = await _context.AuthorizationRecords
            .FirstOrDefaultAsync(m => m.DataAuthorizationNumber == dataauthorizationnumber);
        if (authorizationrecord == null)
        {
            return NotFound();
        }

        return View(authorizationrecord);
    }

    // GET: AUTHORIZATIONRECORDS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: AUTHORIZATIONRECORDS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorizationRecord authorizationrecord)
    {
        if (ModelState.IsValid)
        {
            _context.Add(authorizationrecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(authorizationrecord);
    }

    // GET: AUTHORIZATIONRECORDS/Edit/5
    public async Task<IActionResult> Edit(string? dataauthorizationnumber)
    {
        if (dataauthorizationnumber == null)
        {
            return NotFound();
        }

        var authorizationrecord = await _context.AuthorizationRecords.FindAsync(dataauthorizationnumber);
        if (authorizationrecord == null)
        {
            return NotFound();
        }
        return View(authorizationrecord);
    }

    // POST: AUTHORIZATIONRECORDS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? dataauthorizationnumber, [Bind("DataAuthorizationNumber,PetID,InstitutionID,GrantedDate,RevokedDate,Status,Pet,Institution")] AuthorizationRecord authorizationrecord)
    {
        if (dataauthorizationnumber != authorizationrecord.DataAuthorizationNumber)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(authorizationrecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorizationRecordExists(authorizationrecord.DataAuthorizationNumber))
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
        return View(authorizationrecord);
    }

    // GET: AUTHORIZATIONRECORDS/Delete/5
    public async Task<IActionResult> Delete(string? dataauthorizationnumber)
    {
        if (dataauthorizationnumber == null)
        {
            return NotFound();
        }

        var authorizationrecord = await _context.AuthorizationRecords
            .FirstOrDefaultAsync(m => m.DataAuthorizationNumber == dataauthorizationnumber);
        if (authorizationrecord == null)
        {
            return NotFound();
        }

        return View(authorizationrecord);
    }

    // POST: AUTHORIZATIONRECORDS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? dataauthorizationnumber)
    {
        var authorizationrecord = await _context.AuthorizationRecords.FindAsync(dataauthorizationnumber);
        if (authorizationrecord != null)
        {
            _context.AuthorizationRecords.Remove(authorizationrecord);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AuthorizationRecordExists(string? dataauthorizationnumber)
    {
        return _context.AuthorizationRecords.Any(e => e.DataAuthorizationNumber == dataauthorizationnumber);
    }
}
