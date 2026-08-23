using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using PetCare.Controllers;
using System.Security.Claims;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;
//飼主用controller
//獸醫端另算

public class MedicalRecordController : BaseController
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
        var myRecords = _context.MedicalRecords
            .Include(m => m.Pet)
            .Include(m => m.Vet)
            .Where(m => m.Pet.OwnerID == LoginUserId)
            .OrderByDescending(m=>m.VisitDate)
            .AsQueryable();

        if (petid > 0)
        {
            myRecords = myRecords.Where(r => r.PetID == petid);
        }

        var pet = await _context.Pets.Where(p => p.OwnerID == LoginUserId).ToListAsync();
        ViewBag.noPet = !pet.Any();

        // 建立 SelectList，參數依序為：資料來源、Value 欄位、Text 欄位、當前選中的值
        ViewBag.PetList = new SelectList(pet, "PetID", "Name", petid);
        ViewData["petid"] = petid;

        return View(await myRecords.ToListAsync());
    }

    // GET: MEDICALRECORDS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var medicalrecord = await _context.MedicalRecords
            .Include(m=>m.Pet)
            .Include(m => m.Vet)
            .Include(m=>m.RecordDiagnoses)
            .ThenInclude(rd=> rd.MedicalVocabularyDictionary)
            .Include(m=>m.Prescriptions)
            .Where(m => m.RecordID == id)
            .FirstOrDefaultAsync();
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

    //將上傳檔案的行為抽離成一個方法，方便在所有action中重複使用
    private async Task<string> SaveUploadFile(IFormFile uploadFile)
    {
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
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadFile.FileName);
            // 3. 組合完整實體路徑
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            // 4. 將檔案寫入伺服器
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await uploadFile.CopyToAsync(fileStream);
            }
            // 5. 回傳檔案名稱給前端，前端可以將其存入表單中
            return uniqueFileName;
        }
        return null;
    }

    // post 建立移除檔案的 action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFile(int recordid, bool fromdetails = false)
    {
        // 1. 從資料庫撈出該筆醫療紀錄
        var medicalRecord = await _context.MedicalRecords.FirstOrDefaultAsync(m => m.RecordID == recordid);

        if (medicalRecord == null)
        {
            return NotFound();
        }

        // 2. 確認有檔案紀錄存在
        if (!string.IsNullOrEmpty(medicalRecord.TestResultsFile))
        {
            // 3. 組合實體檔案路徑並刪除伺服器上的檔案
            string filePath = Path.Combine(_env.WebRootPath, "uploads", "mrFiles", medicalRecord.TestResultsFile);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // 4. 將資料庫中該筆紀錄的檔案欄位設為 null，並儲存變更
            medicalRecord.TestResultsFile = null;
            await _context.SaveChangesAsync();
        }
        if (fromdetails) {
            return RedirectToAction(nameof(Details), new { id = recordid });
        }
        // 5. 刪除成功後，重新導向回原本的 Edit 頁面
        // 這裡會觸發 GET: MedicalRecord/Edit?recordid=xxx
        return RedirectToAction(nameof(Edit), new { recordid = recordid });
    }

    // post 建立上傳檔案的 action
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile uploadFile, int recordid , bool fromdetails = false)
    {
        if (uploadFile != null && uploadFile.Length > 0)
        {
            string uniqueFileName = await SaveUploadFile(uploadFile);
            //更新資料庫中的檔案名稱欄位
            var medicalrecord = await _context.MedicalRecords.FindAsync(recordid);
            if (medicalrecord != null)
            {
                medicalrecord.TestResultsFile = uniqueFileName;
                _context.Update(medicalrecord);
                await _context.SaveChangesAsync();
            }
            if (fromdetails)
            {
                return RedirectToAction(nameof(Details), new { id = recordid });
            }
            return RedirectToAction(nameof(Index));
        }
        return BadRequest("No file uploaded.");
    }

    // GET: MEDICALRECORDS/Edit/5
    public async Task<IActionResult> Edit(int? recordid, bool fromDetails = false)
    {
        if (recordid == null)
        {
            return NotFound();
        }

        var medicalrecord = await _context.MedicalRecords
            .Include(mr=>mr.Pet)
            .Include(mr => mr.Vet)
            .FirstOrDefaultAsync(mr=>mr.RecordID==recordid);
        if (medicalrecord == null)
        {
            return NotFound();
        }
        ViewBag.FromDetails = fromDetails;
        return View(medicalrecord);
    }

    // POST: MEDICALRECORDS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? recordid, MedicalRecord medicalrecord, IFormFile? uploadFile, string? existingFileName, bool fromDetails = false)
    {
        if (recordid != medicalrecord.RecordID)
        {
            return NotFound();
        }
        ModelState.Remove("Pet");
        ModelState.Remove("Vet");
        ModelState.Remove("Vet.Name");
        if (ModelState.IsValid)
        {
            try
            {
                // 判斷使用者是否有上傳檔案
                if (uploadFile != null && uploadFile.Length > 0)
                {
                    //若原本檔案存在，先刪除原本的檔案
                    if (!string.IsNullOrEmpty(existingFileName))
                    {
                        // 刪除舊檔案
                        string oldFilePath = Path.Combine(_env.WebRootPath, "uploads", "mrFiles", existingFileName);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    //呼叫 SaveUploadFile 方法來儲存新的檔案
                    string uniqueFileName = await SaveUploadFile(uploadFile);

                    // 5. 將檔案名稱 (或相對路徑) 存入 Model 中，準備寫入資料庫
                    medicalrecord.TestResultsFile = uniqueFileName;
                }
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
            if (fromDetails)
            {
                return RedirectToAction(nameof(Details), nameof(PetController).Replace("Controller", ""), new { petid = medicalrecord.PetID });
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
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var medicalrecord = await _context.MedicalRecords.FindAsync(id);
        if (medicalrecord != null)
        {
            // 刪除上傳的檔案
            var filePath = Path.Combine(_env.WebRootPath, "uploads", "mrFiles", medicalrecord.TestResultsFile);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            

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
