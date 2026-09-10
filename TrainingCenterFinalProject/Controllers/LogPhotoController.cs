using Microsoft.AspNetCore.Mvc;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class LogPhotoController : Controller
{
    private readonly ILogPhotoService _logPhotoService;

    public LogPhotoController(ILogPhotoService logPhotoService)
    {
        _logPhotoService = logPhotoService;
    }

    // GET: LOGPHOTOS?logid=1
    public async Task<IActionResult> Index(int logid = 0)
    {
        var photos = await _logPhotoService.GetPhotosAsync(logid);
        return View(photos);
    }

    public async Task<IActionResult> Details(int? photoid)
    {
        if (photoid == null) return NotFound();

        var logphoto = await _logPhotoService.GetByIdAsync(photoid.Value);
        if (logphoto == null) return NotFound();

        return View(logphoto);
    }

    public IActionResult Create(int logid = 1)
    {
        ViewData["LogID"] = logid;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LogPhoto logphoto, List<IFormFile>? uploadedFiles)
    {
        ModelState.Remove("ImageURL");

        if (!ModelState.IsValid) return View(logphoto);

        var result = await _logPhotoService.CreateAsync(logphoto);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "新增失敗");
            return View(logphoto);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? photoid)
    {
        if (photoid == null) return NotFound();

        var logphoto = await _logPhotoService.GetByIdAsync(photoid.Value);
        if (logphoto == null) return NotFound();

        return View(logphoto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? photoid, [Bind("PhotoID,LogID,ImageURL,CareGroomingLog")] LogPhoto logphoto)
    {
        if (photoid == null) return NotFound();
        if (!ModelState.IsValid) return View(logphoto);

        var result = await _logPhotoService.UpdateAsync(photoid.Value, logphoto);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? photoid)
    {
        if (photoid == null) return NotFound();

        var logphoto = await _logPhotoService.GetByIdAsync(photoid.Value);
        if (logphoto == null) return NotFound();

        return View(logphoto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? photoid)
    {
        if (photoid == null) return NotFound();

        var result = await _logPhotoService.DeleteAsync(photoid.Value);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult UploadPhoto(int logid = 0)
    {
        ViewData["LogID"] = logid;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadPhoto(int logid, List<IFormFile> uploadedFiles)
    {
        var result = await _logPhotoService.UploadPhotosAsync(logid, uploadedFiles);

        if (!result.IsSuccess && result.ErrorMessage == "NO_FILE")
        {
            TempData["ErrorMessage"] = "Please select at least one file to upload.";
            return RedirectToAction("Create", new { logid });
        }

        return RedirectToAction("Details", "CareGroomingLog", new { logid });
    }

    [HttpPost]
    public async Task<IActionResult> DeletePhoto(int photoid)
    {
        var result = await _logPhotoService.DeletePhotoWithFileAsync(photoid);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return Ok();
    }
}