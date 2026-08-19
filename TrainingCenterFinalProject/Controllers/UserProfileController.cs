
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

//[Authorize]
public class UserProfileController : Controller
{
    private readonly PetCareDbContext _context;

    public UserProfileController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: USERPROFILES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.UserProfiles.ToListAsync());
    }

    // GET: USERPROFILES/Details/5
    public async Task<IActionResult> Details(string? userid)
    {
        if (userid == null)
        {
            return NotFound();
        }

        var userprofile = await _context.UserProfiles
            .FirstOrDefaultAsync(m => m.UserID == userid);
        if (userprofile == null)
        {
            return NotFound();
        }

        return View(userprofile);
    }

    // GET: USERPROFILES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: USERPROFILES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserID,Name,Phone,Authorization,Gender,DateOfBirth,Address,JoinDate,Status,Email,CredentialDetails,Employments,Pets,VetMedicalRecords,StaffCareLogs,StaffTaskSchedules")] UserProfile userprofile)
    {
        if (ModelState.IsValid)
        {
            _context.Add(userprofile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(userprofile);
    }

    // GET: USERPROFILES/Edit/5
    public async Task<IActionResult> Edit(string? userid)
    {
        if (userid == null)
        {
            return NotFound();
        }

        var userprofile = await _context.UserProfiles.FindAsync(userid);
        if (userprofile == null)
        {
            return NotFound();
        }
        return View(userprofile);
    }

    // POST: USERPROFILES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? userid, [Bind("UserID,Name,Phone,Authorization,Gender,DateOfBirth,Address,JoinDate,Status,Email,CredentialDetails,Employments,Pets,VetMedicalRecords,StaffCareLogs,StaffTaskSchedules")] UserProfile userprofile)
    {
        if (userid != userprofile.UserID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(userprofile);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfileExists(userprofile.UserID))
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
        return View(userprofile);
    }

    // GET: USERPROFILES/Delete/5
    public async Task<IActionResult> Delete(string? userid)
    {
        if (userid == null)
        {
            return NotFound();
        }

        var userprofile = await _context.UserProfiles
            .FirstOrDefaultAsync(m => m.UserID == userid);
        if (userprofile == null)
        {
            return NotFound();
        }

        return View(userprofile);
    }

    // POST: USERPROFILES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? userid)
    {
        var userCred = await _context.UserCredentialDetails.Where(ucd=>ucd.UserID == userid).FirstOrDefaultAsync();
        var userprofile = await _context.UserProfiles.FindAsync(userid);
        if (userprofile != null)
        {
            _context.UserCredentialDetails.Remove(userCred);
            _context.UserProfiles.Remove(userprofile);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserProfileExists(string? userid)
    {
        return _context.UserProfiles.Any(e => e.UserID == userid);
    }
}
