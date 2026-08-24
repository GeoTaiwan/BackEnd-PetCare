
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCare.Controllers;
using System.Security.Claims;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;
public class InstitutionController : BaseController
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
            .Include(i=>i.Owner)
            .Where(i=> i.OwnerID == LoginUserId)
            .ToListAsync();
        return View(institutions);
    }
    //使用者查詢可預約機構用
    public async Task<IActionResult> IndexForReservation(string? typeId)
    {
        // 1. 建立一個 IQueryable 查詢基底，先不要直接 ToList()，這樣效能最好
        var query = _context.Institutions.AsQueryable();

        // 2. 如果使用者有傳入篩選條件，就加上 Where 過濾
        if (!string.IsNullOrEmpty(typeId))
        {
            query = query.Where(i => i.TypeId == typeId);
        }

        // 3. 執行查詢並把資料拉回記憶體
        var institutions = await query.ToListAsync();

        // 4. 將目前的篩選條件存入 ViewBag，為了讓前端畫面能維持「目前選取」的狀態
        ViewBag.CurrentType = typeId;

        return View(institutions);
    }

    public IActionResult InstitutionManage(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var institution = _context.Institutions
            .Include(i => i.InstitutionStatus)
            .Include(i => i.InstitutionType)
            .Include(i => i.Owner)
            .Include(i => i.Employments)
            .ThenInclude(e => e.UserProfile)
            .FirstOrDefault(m => m.InstitutionID == id);
        if (institution == null)
        {
            return NotFound();
        }
        return View(institution);
    }

    public IActionResult Details(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var institution = _context.Institutions
            .Include(i => i.InstitutionStatus)
            .Include(i => i.InstitutionType)
            .Include(i => i.Owner)
            .FirstOrDefault(m => m.InstitutionID == id);
        if (institution == null)
        {
            return NotFound();
        }
        return View(institution);
    }

    // GET: INSTITUTIONS/Create
    public IActionResult Create()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        //到資料庫查詢是否機構已在申請中
        var institution = _context.Institutions.FirstOrDefault(i => i.OwnerID == currentUserId && i.StatusId == "S00");
        bool alreadyApplying = institution != null;
        if (alreadyApplying)
        {
            //TempData["ErrorMessage"] = "機構正在申請中。";
            return RedirectToAction("Details",new { id = institution.InstitutionID });
        }
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
        if (institution == null) return BadRequest();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 商業規則：一人只能擔任一間機構的負責人
        bool alreadyOwns = await _context.Employments.AnyAsync(e =>
            e.UserID == currentUserId &&
            e.OccupationID == "O00" &&
            e.Status == "Active");

        if (alreadyOwns)
        {
            ModelState.AddModelError("", "您已經是某機構的負責人，無法重複申請。");
            PopulateInstitutionTypesAndStatuses();
            return View(institution);
        }

        ModelState.Remove("InstitutionID"); // 系統自動產生
        ModelState.Remove("StatusId");      // 不採用表單輸入，一律進審核
        ModelState.Remove("OwnerID");       // 不採用表單輸入，讀取登入者資訊
        institution.StatusId = "S00";       // 審核中
        institution.OwnerID = currentUserId;
        if (!ModelState.IsValid)
        {
            PopulateInstitutionTypesAndStatuses();
            ModelState.AddModelError("", "請確認所有欄位皆已正確填寫。");
            return View(institution);
        }

        // 重試機制：處理 InstitutionID 併發衝突（原本 GenerateInstitutionID 沒有防護）
        const int maxRetries = 3;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            institution.InstitutionID = await GenerateInstitutionIDAsync();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Institutions.Add(institution);
                await _context.SaveChangesAsync();

                _context.Employments.Add(new Employment
                {
                    UserID = currentUserId,
                    InstitutionID = institution.InstitutionID,
                    OccupationID = "O00",
                    Status = "Active",
                    LicenseNumber = institution.LicenseNumber ?? "PENDING", // 見下方說明(管理者不一定有執照號碼)
                    StartDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                //TempData["SuccessMessage"] = "機構申請已送出，請等待管理員審核。";
                return RedirectToAction("Index","Home");
            }
            catch (DbUpdateException) when (attempt < maxRetries - 1)
            {
                await transaction.RollbackAsync();
                _context.Entry(institution).State = EntityState.Detached;
                // 迴圈繼續，重新產生 ID 再試一次
            }
        }

        ModelState.AddModelError("", "系統忙碌，請稍後再試一次。");
        PopulateInstitutionTypesAndStatuses();
        return View(institution);
    }

    // 原本的 GenerateInstitutionID() 改成 async 版本
    private async Task<string> GenerateInstitutionIDAsync()
    {
        var lastInstitution = await _context.Institutions
            .OrderByDescending(i => i.InstitutionID)
            .FirstOrDefaultAsync();

        if (lastInstitution == null) return "I00001";

        int lastIDNumber = int.Parse(lastInstitution.InstitutionID.Substring(1));
        return "I" + (lastIDNumber + 1).ToString("D5");
    }

    // GET: INSTITUTIONS/Edit/5
    
    public async Task<IActionResult> Edit(string? id, bool fromDetails = false)
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
        ViewBag.FromDetails = fromDetails; // 傳遞參數到 View
        return View(institution);
    }

    // POST: INSTITUTIONS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Institution institution, bool fromDetails = false)
    {
        if (institution == null)
        {
            return NotFound("1");
        }
        
        if (fromDetails)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ModelState.Remove("InstitutionID");
            ModelState.Remove("StatusId");
            ModelState.Remove("OwnerID");
            institution.StatusId = "S00"; // 審核中
            institution.OwnerID = currentUserId;
            institution.InstitutionID = await _context.Institutions.Where(i => i.OwnerID == currentUserId).Select(i => i.InstitutionID).FirstOrDefaultAsync(); // 保留原本的 InstitutionID
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
                    return NotFound("3");
                }
                else
                {
                    throw;
                }
            }
            if (fromDetails)
            {
                return RedirectToAction("Details", new { id = institution.InstitutionID });
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
