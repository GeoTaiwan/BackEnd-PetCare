
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Data;

public class LogPhotoController : Controller
{
    private readonly PetCareDbContext _context;
    private readonly IWebHostEnvironment _env;
    public LogPhotoController(PetCareDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
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
    public IActionResult Create(int logid=1)
    {
        //uploads/logPhotos/
        ViewData["LogID"] = logid;
        return View();
    }

    // POST: LOGPHOTOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LogPhoto logphoto,List<IFormFile>? uploadedFiles)
    {
        ModelState.Remove("ImageURL");
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

    public IActionResult UploadPhoto(int logid=0)
    {
        ViewData["LogID"] = logid;
        return View();
    }

    //新增照片上傳功能
    [HttpPost]
    public async Task<IActionResult> UploadPhoto(int logid, List<IFormFile> uploadedFiles)
    {
        if (uploadedFiles == null || uploadedFiles.Count == 0)
        {
            TempData["ErrorMessage"] = "Please select at least one file to upload.";
            //ModelState.AddModelError("", "Please select at least one file to upload.");
            return RedirectToAction("Create", new { logid = logid });
        }
        foreach (var file in uploadedFiles)
        {
            if (file.Length > 0)
            {
                // Generate a unique filename
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/logPhotos");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Create a new LogPhoto entry in the database
                var logPhoto = new LogPhoto
                {
                    LogID = logid,
                    ImageURL = "/uploads/logPhotos/" + uniqueFileName
                };
                _context.LogPhotos.Add(logPhoto);
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "CareGroomingLog", new { id = logid });
    }
    //自訂刪除照片功能，刪除資料庫紀錄與實體檔案
    //返迴到CareGroomingLog的Details頁面
    [HttpPost]
    public async Task<IActionResult> DeletePhoto(int photoid)
    {
        var logPhoto = await _context.LogPhotos.FindAsync(photoid);
        if (logPhoto == null)
        {
            return NotFound();
        }
        // Delete the file from the server
        var filePath = Path.Combine(_env.WebRootPath, logPhoto.ImageURL.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        // Remove the LogPhoto entry from the database
        _context.LogPhotos.Remove(logPhoto);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "CareGroomingLog", new { id = logPhoto.LogID });
    }

}
