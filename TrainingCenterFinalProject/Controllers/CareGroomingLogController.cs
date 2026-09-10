using Microsoft.AspNetCore.Mvc;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class CareGroomingLogController : BaseController
{
    private readonly ICareGroomingLogService _careGroomingLogService;

    public CareGroomingLogController(ICareGroomingLogService careGroomingLogService)
    {
        _careGroomingLogService = careGroomingLogService;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var careLogs = await _careGroomingLogService.GetLogsForUserAsync(LoginUserId);
        return View(careLogs);
    }

    public async Task<IActionResult> Details(int? logid)
    {
        if (logid == null) return NotFound();

        var caregroominglog = await _careGroomingLogService.GetDetailsAsync(logid.Value);
        if (caregroominglog == null) return NotFound();

        return View(caregroominglog);
    }

    public async Task<IActionResult> Create(int petid = 0)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var data = await _careGroomingLogService.GetCreateDataAsync(LoginUserId, petid);

        if (data.ShouldRedirectToPets)
        {
            TempData["ErrorMessage"] = data.ErrorMessage;
            return RedirectToAction("Index", "Pet");
        }

        return View(data.Log);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("LogID,PetID,LogDate,ActivityDetails,DietNotes")] CareGroomingLog caregroominglog)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        ModelState.Remove("StaffID");
        ModelState.Remove("Pet");
        ModelState.Remove("Staff");
        ModelState.Remove("LogPhotos");

        if (!ModelState.IsValid) return View(caregroominglog);

        var result = await _careGroomingLogService.CreateAsync(caregroominglog, LoginUserId);
        if (!result.IsSuccess) return View(caregroominglog);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? logid, bool fromDetails = false)
    {
        if (logid == null) return NotFound();

        var caregroominglog = await _careGroomingLogService.GetEditDataAsync(logid.Value);
        if (caregroominglog == null) return NotFound();

        ViewBag.FromDetails = fromDetails;
        return View(caregroominglog);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? logid, [Bind("LogID,PetID,LogDate,ActivityDetails,DietNotes")] CareGroomingLog caregroominglog, bool fromDetails = false)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();
        if (logid == null) return NotFound();

        ModelState.Remove("StaffID");
        ModelState.Remove("Pet");
        ModelState.Remove("Staff");
        ModelState.Remove("LogPhotos");

        if (!ModelState.IsValid) return View(caregroominglog);

        var result = await _careGroomingLogService.UpdateAsync(logid.Value, caregroominglog, LoginUserId);

        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();
        if (!result.IsSuccess && result.ErrorMessage == "FORBIDDEN") return Forbid();

        if (fromDetails)
        {
            return RedirectToAction("Details", new { logid = caregroominglog.LogID });
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? logid)
    {
        if (logid == null) return NotFound();

        var caregroominglog = await _careGroomingLogService.GetDeleteDataAsync(logid.Value);
        if (caregroominglog == null) return NotFound();

        return View(caregroominglog);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? logid)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();
        if (logid == null) return NotFound();

        var result = await _careGroomingLogService.DeleteAsync(logid.Value, LoginUserId);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();
        if (!result.IsSuccess && result.ErrorMessage == "FORBIDDEN") return Forbid();

        return RedirectToAction(nameof(Index));
    }
}