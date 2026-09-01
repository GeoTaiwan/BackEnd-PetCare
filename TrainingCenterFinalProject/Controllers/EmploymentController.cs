
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.Controllers;
using PetCare.ViewModels;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;
[Authorize(Roles = "Vet,Beauty,Nanny,VetManage,BeautyManage,NannyManage")]
public class EmploymentController : BaseController
{
    private readonly PetCareDbContext _context;

    public EmploymentController(PetCareDbContext context)
    {
        _context = context;
    }

    private async Task<string?> GetInstitutionIdByUserId(string userId)
    {
        return await _context.Employments.Where(e => e.UserID == userId).Select(e => e.InstitutionID).FirstOrDefaultAsync();
    }

    // GET: EMPLOYMENTS
    public async Task<IActionResult> Index()
    {
        string? instID = await GetInstitutionIdByUserId(LoginUserId);
        var employments = Enumerable.Empty<Employment>().AsQueryable();

        if (!string.IsNullOrEmpty(instID))
        {
            employments = _context.Employments
            .Include(e => e.UserProfile)
            .Include(e => e.OccupationDetails)
            .Include(e => e.Institution)
            .Where(e => e.InstitutionID == instID)
            .AsQueryable();
            employments = employments.Where(e => e.InstitutionID == instID).OrderBy(e => e.StartDate);
        } else
        {
            return BadRequest();
        }
        ViewBag.InstitutionName = employments.Select(e => e.Institution.Name).FirstOrDefault();

        return View(await employments.ToListAsync());
    }


    // GET: EMPLOYMENTS/Create
    public async Task<IActionResult> Create(string instid,bool fromInstManage=false)
    {
        var employ = new NewEmployeeViewModel
        {
            InstitutionID=instid,
            LicenseNumber="待填",
            InstitutionName = await _context.Institutions.Where(i => i.InstitutionID == instid).Select(i => i.Name).FirstOrDefaultAsync(),
            Institution = await _context.Institutions.FindAsync(instid)
        };
        ViewBag.fromInstManage = fromInstManage;
        await getList();
        return View(employ);
    }

    //實作方法：職位代號、在職狀態傳入下拉式選單
    private async Task getList()
    {
        var occ = await _context.Occupations.ToListAsync();
        ViewBag.OccupationList = new SelectList(occ, "OccupationID", "OccupationName");
        var statusList = new[]
        {
            new { Value = "Active", Text = "在職 (Active)" },
            new { Value = "Inactive", Text = "離職 (Inactive)" },
            new { Value = "Suspended", Text = "停職 (Suspended)" } // 可隨時擴充
        };

        ViewBag.StatusList = new SelectList(statusList,"Value","Text");
    } 

    // POST: EMPLOYMENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewEmployeeViewModel emViewModel,bool fromInstManage=false)
    {
        emViewModel.StartDate = DateTime.Now;
        if (ModelState.IsValid)
        {
            var employment = new Employment
            {
                UserID = await _context.UserProfiles.Where(u=>u.Email==emViewModel.UserEmail).Select(u=>u.UserID).FirstOrDefaultAsync(),
                InstitutionID = emViewModel.InstitutionID,
                OccupationID=emViewModel.OccupationID,
                Status=emViewModel.Status,
                LicenseNumber=emViewModel.LicenseNumber,
                StartDate=emViewModel.StartDate
            };
            var user = await _context.UserProfiles.FindAsync(employment.UserID);
            if (employment.OccupationID == "O00")
            {
                user.Authorization = 4; // 設定使用者權限為管理者
            }
            else
            {
                user.Authorization = 1; // 設定使用者權限為一般員工
            }
            _context.Add(employment);
            await _context.SaveChangesAsync();
            if (fromInstManage) {
                return RedirectToAction("InstitutionManage", "Institution", new {id = emViewModel.InstitutionID });
            }
            return RedirectToAction(nameof(Index));
        }
        await getList();
        return View(emViewModel);
    }

    // GET: EMPLOYMENTS/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        string? instID = await GetInstitutionIdByUserId(LoginUserId);
        var employment = await _context.Employments
            .Include(u => u.Institution)
            .Include(u => u.UserProfile)
            .Where(e => e.UserID == id && e.InstitutionID == instID)
            .FirstOrDefaultAsync();
        if (employment == null)
        {
            return NotFound();
        }
        await getList();
        
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
                var user = await _context.UserProfiles.FindAsync(employment.UserID);
                if (employment.Status == "Inactive")
                {
                    employment.EndDate = DateTime.Now;
                    user.Authorization = 0; // 設定使用者權限為一般使用者
                }
                else if (employment.Status == "Active")
                {
                    employment.EndDate = null;
                    if (employment.OccupationID == "O00")
                    {
                        user.Authorization = 4; // 設定使用者權限為管理者
                    }
                    else
                    {
                        user.Authorization = 1; // 設定使用者權限為一般員工
                    }
                }
                else {
                    user.Authorization = 0; // 設定使用者權限為一般使用者
                }

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
