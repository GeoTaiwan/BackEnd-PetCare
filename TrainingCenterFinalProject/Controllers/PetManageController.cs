
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using PetCare.Controllers;
using System.Security.Claims;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;
[Authorize(Roles = "VetManage,BeautyManage,NannyManage" +
    ",Vet,Beauty,Nanny")]
public class PetManageController : BaseController
{
    private readonly PetCareDbContext _context;
    private readonly IWebHostEnvironment _env; // 💡 1. 宣告環境變數

    public PetManageController(PetCareDbContext context, IWebHostEnvironment env)

    {
        _context = context;
        _env = env;
    }

    // GET: PETS
    public async Task<IActionResult> Index()
    {
        // 取得登入使用者的機構代碼
        var instiID = await _context.Employments
            .Where(e => e.UserID == LoginUserId)
            .Select(e => e.InstitutionID)
            .FirstOrDefaultAsync();
        var inst = await _context.Institutions
            .Include(i => i.InstitutionType)
            .Where(i => i.InstitutionID == instiID)
            .FirstOrDefaultAsync();

        var instType = inst.InstitutionType;
        var instPets = _context.Pets.AsQueryable();
        bool isVet = instType.Name == "動物醫院";
        if (isVet)
        {
            instPets = instPets
                .Include(p => p.MedicalRecords)
                .AsQueryable();
        }
        else
        {
            instPets = instPets
                .Include(p => p.CareGroomingLogs)
                .AsQueryable();
        }
        ViewBag.isVet = isVet;
        ViewBag.institutionID = instiID;
        ViewBag.institutionName = inst.Name;
        // 用 InstitutionID 去過濾寵物，僅顯示在該機構有預約的寵物
        // AuthorizationRecords 也要過濾，只顯示目前有效的授權紀錄 (GrantedDate < Now && RevokedDate >= Now)
        // 且institutionID要與Appointment的InstitutionID相同
        instPets = instPets
            .Include(i => i.Appointments).ThenInclude(a => a.Institution)
            .Include(i => i.AuthorizationRecords
            .Where(ar => ar.GrantedDate < DateTime.Now &&
            ar.RevokedDate >= DateTime.Now &&
            ar.InstitutionID == instiID))
            .Where(p => p.Appointments.Any(a => a.InstitutionID == instiID))
            .OrderByDescending(p => p.Appointments.Max(a => a.ApptDateTime));


        return View(await instPets.ToListAsync());
    }

    // GET: PETS/Details/5
    public async Task<IActionResult> Details(int? petid)
    {
        if (petid == null)
        {
            return NotFound();
        }

        var pet = await _context.Pets
            .Include(p => p.PetWeights.OrderByDescending(w => w.RecordDate))
            .Include(p => p.MedicalRecords.OrderByDescending(m => m.VisitDate))
            .Include(p => p.CareGroomingLogs.OrderByDescending(l => l.LogDate)).ThenInclude(l => l.Staff) //關聯CareGroomingLog的Staff
            .Include(p => p.Appointments.OrderByDescending(a => a.ApptDateTime)).ThenInclude(a => a.Institution) //關聯Appointments
            .Include(p=>p.AuthorizationRecords)
            .FirstOrDefaultAsync(m => m.PetID == petid);
        if (pet == null)
        {
            return NotFound();
        }

        return View(pet);
    }

    // GET: PETS/Create
    public IActionResult Create(bool fromMR = false)
    {
        ViewBag.FromMR = fromMR;
        return View();
    }

    // POST: PETS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PetID,Name,Species,Breed,Gender,DateOfBirth,MicrochipNo,SpecialNotes,UploadPhoto")] Pet pet, bool fromMR = false)
    {
        // 💡 1. 從 User Claims 抓取當前登入者的 UserID
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 💡 2. 強制將登入者的 ID 寫入寵物的 OwnerID (防竄改)
        pet.OwnerID = currentUserId;

        // 💡 3. 移除 ModelState 中對 OwnerID 的驗證錯誤 (因為前端表單沒有傳這個欄位，原本會被判定為無效)
        ModelState.Remove("OwnerID");

        if (ModelState.IsValid)
        {
            // 💡 步驟 1：先儲存文字資料，讓資料庫正式配發 PetID
            _context.Add(pet);
            await _context.SaveChangesAsync();
            // 執行完上面這行後，pet.PetID 就會從 0 變成真正的數字 (例如 15) 了！

            // 💡 步驟 2：處理照片，並使用剛拿到的 PetID 來命名
            if (pet.UploadPhoto != null && pet.UploadPhoto.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "pets");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 取得原本的副檔名 (包含點，例如 ".jpg", ".png")
                string extension = Path.GetExtension(pet.UploadPhoto.FileName);

                // 組合新檔名：UserID_PetID.副檔名 (例如：U000000000001_15.jpg)
                string newFileName = $"{currentUserId}_{pet.PetID}{extension}";

                string filePath = Path.Combine(uploadsFolder, newFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await pet.UploadPhoto.CopyToAsync(fileStream);
                }

                // 💡 步驟 3：更新圖片路徑並再次儲存到資料庫
                pet.ProfilePhotoUrl = "/uploads/pets/" + newFileName;
                _context.Update(pet);
                await _context.SaveChangesAsync();
            }
            if (fromMR)
            {
                return RedirectToAction(nameof(Index), nameof(MedicalRecordManageController).Replace("Controller", ""));
            }
            return RedirectToAction(nameof(Index));
        }

        return View(pet); // 失敗的話退回表單
    }

    // GET: PETS/Edit/5
    public async Task<IActionResult> Edit(int? petid, bool fromdetails = false)
    {
        if (petid == null) return NotFound();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 💡 安全防護：不僅要找 PetID，還必須核對 OwnerID
        var pet = await _context.Pets
            .FirstOrDefaultAsync(p => p.PetID == petid && p.OwnerID == currentUserId);

        if (pet == null) return NotFound(); // 如果找不到，或根本不是他的寵物，就回傳 404
        ViewBag.FromDetails = fromdetails;
        ViewBag.PetID = petid;
        return View(pet);
    }

    // POST: PETS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? petid, [Bind("PetID,Name,Species,Breed,Gender,DateOfBirth,MicrochipNo,SpecialNotes,ProfilePhotoUrl,UploadPhoto")] Pet pet, bool fromdetails = false)
    {
        if (petid != pet.PetID) return NotFound();

        // 💡 1. 抓取當前使用者
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 💡 2. 強制綁定擁有者 (防止透過改網頁原始碼竄改 OwnerID)
        pet.OwnerID = currentUserId;
        ModelState.Remove("OwnerID");

        if (ModelState.IsValid)
        {
            try
            {
                // 💡 處理新照片上傳與舊照片刪除邏輯
                if (pet.UploadPhoto != null && pet.UploadPhoto.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "pets");

                    // 【防護機制】：如果有舊照片，先去實體硬碟把它刪除
                    if (!string.IsNullOrEmpty(pet.ProfilePhotoUrl))
                    {
                        // 抓出舊檔案的純檔名 (例如 U001_15.jpg)
                        string oldFileName = Path.GetFileName(pet.ProfilePhotoUrl);
                        // 組合成實體硬碟路徑 (C:\...\wwwroot\uploads\pets\U001_15.jpg)
                        string exactOldFilePath = Path.Combine(uploadsFolder, oldFileName);

                        // 檢查檔案是否存在，存在就刪除
                        if (System.IO.File.Exists(exactOldFilePath))
                        {
                            System.IO.File.Delete(exactOldFilePath);
                        }
                    }

                    // 【寫入新檔案】
                    string extension = Path.GetExtension(pet.UploadPhoto.FileName);
                    string newFileName = $"{currentUserId}_{pet.PetID}{extension}";
                    string filePath = Path.Combine(uploadsFolder, newFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await pet.UploadPhoto.CopyToAsync(fileStream);
                    }

                    // 更新資料庫中的路徑為新照片
                    pet.ProfilePhotoUrl = "/uploads/pets/" + newFileName;
                }
                // ⚠️ 注意：如果 pet.UploadPhoto 是 null，
                // 因為前端有隱藏的 <input type="hidden" asp-for="ProfilePhotoUrl" />，
                // pet.ProfilePhotoUrl 會保留原本的值，不會被清空！
                _context.Update(pet);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetExists(pet.PetID)) return NotFound();
                else throw;
            }
            if (fromdetails)
            {
                return RedirectToAction(nameof(Details), nameof(PetController).Replace("Controller", ""), new { petid = pet.PetID });
            }
            return RedirectToAction(nameof(Index));
        }
        return View(pet);
    }
    private bool PetExists(int id)
    {
        return _context.Pets.Any(e => e.PetID == id);
    }



    // POST: PETS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? petid)
    {
        if (petid == null)
        {
            return NotFound();
        }
        var pet = await _context.Pets.FindAsync(petid);

        if (pet != null)
        {
            // 💡 新增：刪除寵物前，先檢查並刪除實體照片
            if (!string.IsNullOrEmpty(pet.ProfilePhotoUrl))
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "pets");
                string fileName = Path.GetFileName(pet.ProfilePhotoUrl);
                string filePath = Path.Combine(uploadsFolder, fileName);

                // 如果硬碟裡真的有這個檔案，就把它刪除
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            var petWeights = await _context.PetWeight.Where(w => w.PetID == petid).ToListAsync();
            var medRecord = await _context.MedicalRecords.Where(m => m.PetID == petid).ToListAsync();
            //var careLog = await _context.CareGroomingLogs.Where(cg => cg.PetID == petid).ToListAsync();
            if (petWeights.Any())
            {
                _context.PetWeight.RemoveRange(petWeights);
            }
            if (medRecord.Any())
            {
                _context.MedicalRecords.RemoveRange(medRecord);
            }
            //if (careLog.Any())
            //{
            //    _context.CareGroomingLogs.RemoveRange(careLog);
            //}
            _context.Pets.Remove(pet);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int petid)
    {
        // 💡 1. 取得當前登入者
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 💡 2. 安全防護：不僅找 PetID，還要確認 OwnerID 是當前使用者
        var pet = await _context.Pets
            .FirstOrDefaultAsync(p => p.PetID == petid && p.OwnerID == currentUserId);
        if (pet != null)
        {
            if (!string.IsNullOrEmpty(pet.ProfilePhotoUrl))
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "pets");
                string fileName = Path.GetFileName(pet.ProfilePhotoUrl);
                string filePath = Path.Combine(uploadsFolder, fileName);

                // 如果硬碟裡真的有這個檔案，就把它刪除
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            pet.ProfilePhotoUrl = null;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Edit), new { petid = petid });
    }

    private bool PetExists(int? petid)
    {
        return _context.Pets.Any(e => e.PetID == petid);
    }
}
