
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Controllers;
using System.Security.Claims;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

public class CareGroomingLogController : BaseController
{
    private readonly PetCareDbContext _context;
    //private readonly IWebHostEnvironment _env;
    
    public CareGroomingLogController(PetCareDbContext context)
    {
        _context = context;
        
        //_env = env;
    }

   

    // GET: CAREGROOMINGLOGS
    public async Task<IActionResult> Index()    
    {
        var careLogs = await _context.CareGroomingLogs
            .Include(cg => cg.Pet)
            .ThenInclude(p => p.Owner)
            .Include(cg=>cg.Staff)
            .Where(cg => cg.Pet.OwnerID == LoginUserId) 
            .ToListAsync();
        //若目前使用者為照護人員，則顯示所有經手照護紀錄
        //var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserID == _loginUserId);
        var userEmployments = await _context.Employments
        .Where(e => e.UserID == LoginUserId)
        .ToListAsync();
        var employeeProfile = userEmployments.FirstOrDefault(e =>
        !string.IsNullOrEmpty(e.OccupationID) &&
        e.OccupationID.Length >= 3 &&
        int.TryParse(e.OccupationID.Substring(1, 2), out int occId) &&
        occId > 10);
        if (employeeProfile != null) 
        {
            careLogs = await _context.CareGroomingLogs
                .Where(cg => cg.StaffID == LoginUserId)
                .Include(cg => cg.Pet)
                .ThenInclude(p => p.Owner)
                .Include(cg => cg.Staff)
                .ToListAsync();
        }
        return View(careLogs);
    }

    // GET: CAREGROOMINGLOGS/Details/5
    public async Task<IActionResult> Details(int? logid)
    {
        if (logid == null)
        {
            return NotFound();
        }

        var caregroominglog = await _context.CareGroomingLogs
            .Include(cg => cg.Pet)
            .Include(cg => cg.Staff)
            .Include(cg => cg.LogPhotos)
            .FirstOrDefaultAsync(m => m.LogID == logid);
        if (caregroominglog == null)
        {
            return NotFound();
        }

        return View(caregroominglog);
    }

    // GET: CAREGROOMINGLOGS/Create
    public async Task<IActionResult> Create(int petid=0)
    {
        //找出登入的使用者ID
        var loginuserid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userPets = await _context.Pets.Where(p => p.OwnerID == loginuserid).ToListAsync();
        if (petid == 0)
        {
            if (!userPets.Any())
            {
                //使用者沒有寵物，導向到寵物列表頁面
                TempData["ErrorMessage"] = "您尚未新增寵物，請先新增寵物後再新增照護紀錄。";
                return RedirectToAction("Index", "Pets");
            }
            else
            {                 //使用者有寵物，預設選擇第一隻寵物
                petid = userPets.First().PetID;
            }

        }
        var log = new CareGroomingLog
        {
            PetID = petid
        };
        return View(log);
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
    public async Task<IActionResult> Edit(int? logid, bool fromDetails = false)
    {
        if (logid == null)
        {
            return NotFound();
        }

        var caregroominglog = await _context.CareGroomingLogs
        .Include(cg => cg.Staff)         // 關聯 員工資料 (UserProfiles)
        .Include(cg => cg.Pet)           // 關聯 寵物資料 (Pets)
        .Include(cg => cg.LogPhotos)     // 關聯 多張照片資料 (LogPhotos)
        .FirstOrDefaultAsync(cg => cg.LogID == logid); // 替代原本的 FindAsync
        if (caregroominglog == null)
        {
            return NotFound();
        }
        ViewBag.FromDetails = fromDetails;
        return View(caregroominglog);
    }

    // POST: CAREGROOMINGLOGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? logid, [Bind("LogID,PetID,StaffID,LogDate,ActivityDetails,DietNotes,Pet,Staff,LogPhotos")] CareGroomingLog caregroominglog, bool fromDetails = false)
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
            if (fromDetails)
            {
                return RedirectToAction("Details", new { logid = caregroominglog.LogID });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
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
