using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Security.Claims;
using TrainingCenterFinalProject.Models;
//飼主用controller
//獸醫端另算

public class MedicalRecordController : Controller
{
    private readonly PetCareDbContext _context;
    private readonly IWebHostEnvironment _env;

    public MedicalRecordController(PetCareDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: MEDICALRECORDS
    public async Task<IActionResult> Index(int petid = 0)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var myRecords = _context.MedicalRecords
            .Include(m => m.Pet)
            .Include(m => m.Vet)
            .Where(m => m.Pet.OwnerID == currentUserId)
            .OrderByDescending(m=>m.VisitDate)
            .AsQueryable();

        if (petid > 0)
        {
            myRecords = myRecords.Where(r => r.PetID == petid);
        }

        var pet = await _context.Pets.Where(p => p.OwnerID == currentUserId).ToListAsync();
        ViewBag.noPet = !pet.Any();

        // 建立 SelectList，參數依序為：資料來源、Value 欄位、Text 欄位、當前選中的值
        ViewBag.PetList = new SelectList(pet, "PetID", "Name", petid);
        ViewData["petid"] = petid;

        return View(await myRecords.ToListAsync());
    }

    // GET: MEDICALRECORDS/Details/5
    public async Task<IActionResult> Details(int? recordid)
    {
        if (recordid == null)
        {
            return NotFound();
        }

        var medicalrecord = await _context.MedicalRecords
            .FirstOrDefaultAsync(m => m.RecordID == recordid);
        if (medicalrecord == null)
        {
            return NotFound();
        }

        return View(medicalrecord);
    }

    // GET: MEDICALRECORDS/Create
    public async Task<IActionResult> Create(int petid = 0, bool fromDetails = false)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var pet = await _context.Pets.Where(p => p.OwnerID == currentUserId).ToListAsync();
        if (pet != null)
        {
            if (petid == 0)
            {
                petid = pet.FirstOrDefault().PetID;
            }
            //var pet = await _context.Pets.Where(p => p.OwnerID == currentUserId).ToListAsync();
            ViewBag.PetList = new SelectList(pet, "PetID", "Name", petid);
            ViewBag.FromDetails = fromDetails;
            ViewBag.PetID = petid;
            return View();
        }
        return NotFound();
    }

    // POST: MEDICALRECORDS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedicalRecord medicalrecord, IFormFile? uploadFile,bool fromDetails=false)
    {
        string defaultVet = "U202607280002";
        medicalrecord.VetID = defaultVet;
        ModelState.Remove(nameof(medicalrecord.VetID));
        var pet = await _context.Pets.ToListAsync();
        if (ModelState.IsValid)
        {
            // 判斷使用者是否有上傳檔案
            if (uploadFile != null && uploadFile.Length > 0)
            {
                // 1. 設定存檔路徑 (wwwroot/upload/mrFiles)
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "mrFiles");

                // 如果資料夾不存在，則建立它
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 2. 產生唯一的檔案名稱 (避免檔名重複覆蓋)
                // 例如: 5b6c8f9d-1234-abcd_原本檔名.pdf
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadFile.FileName);

                // 3. 組合完整實體路徑
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // 4. 將檔案寫入伺服器
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(fileStream);
                }

                // 5. 將檔案名稱 (或相對路徑) 存入 Model 中，準備寫入資料庫
                medicalrecord.TestResultsFile = uniqueFileName;
            }
            _context.Add(medicalrecord);
            await _context.SaveChangesAsync();
            if (fromDetails)
            {
                return RedirectToAction(nameof(Details),nameof(PetController).Replace("Controller",""),new {petid=medicalrecord.PetID });
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.PetList = new SelectList(pet, "PetID", "Name", medicalrecord.PetID);
        return View(medicalrecord);
    }

    // GET: MEDICALRECORDS/Edit/5
    public async Task<IActionResult> Edit(int? recordid)
    {
        if (recordid == null)
        {
            return NotFound();
        }

        var medicalrecord = await _context.MedicalRecords.FindAsync(recordid);
        if (medicalrecord == null)
        {
            return NotFound();
        }
        return View(medicalrecord);
    }

    // POST: MEDICALRECORDS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? recordid, [Bind("RecordID,PetID,VetID,VisitDate,Symptoms,TestResultsFile,Diagnosis,Pet,Vet,RecordDiagnoses,Prescriptions")] MedicalRecord medicalrecord)
    {
        if (recordid != medicalrecord.RecordID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(medicalrecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalRecordExists(medicalrecord.RecordID))
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
        return View(medicalrecord);
    }

    // GET: MEDICALRECORDS/Delete/5
    public async Task<IActionResult> Delete(int? recordid)
    {
        if (recordid == null)
        {
            return NotFound();
        }

        var medicalrecord = await _context.MedicalRecords
            .FirstOrDefaultAsync(m => m.RecordID == recordid);
        if (medicalrecord == null)
        {
            return NotFound();
        }

        return View(medicalrecord);
    }

    // POST: MEDICALRECORDS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? recordid)
    {
        var medicalrecord = await _context.MedicalRecords.FindAsync(recordid);
        if (medicalrecord != null)
        {
            _context.MedicalRecords.Remove(medicalrecord);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public IActionResult DownloadFile(string fileName)
    {
        // 1. 檢查檔名是否為空
        if (string.IsNullOrEmpty(fileName))
        {
            return NotFound("沒有提供檔名。");
        }

        // 2. 組合出實體的完整路徑
        string filePath = Path.Combine(_env.WebRootPath, "uploads", "mrFiles", fileName);

        // 3. 檢查實體檔案到底存不存在
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound($"伺服器上找不到檔案: {fileName}");
        }

        // 4. 回傳檔案 (application/octet-stream 代表強制下載)
        return PhysicalFile(filePath, "application/octet-stream", fileName);
    }
    private bool MedicalRecordExists(int? recordid)
    {
        return _context.MedicalRecords.Any(e => e.RecordID == recordid);
    }
}
