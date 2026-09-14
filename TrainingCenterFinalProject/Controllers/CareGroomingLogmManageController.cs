using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;
namespace TrainingCenterFinalProject.Controllers;

[Authorize(Roles = "BeautyManage,NannyManage,Beauty,Nanny")]
public class CareGroomingLogManageController : BaseController
{
    private readonly ICareGroomingLogManageService _careGroomingLogManageService;
    private readonly IUserProfileService _userProfileService;

    public CareGroomingLogManageController(ICareGroomingLogManageService careGroomingLogManageService, IUserProfileService userProfileService)
    {
        _careGroomingLogManageService = careGroomingLogManageService;
        _userProfileService = userProfileService;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();
        var user = await _userProfileService.GetByIdAsync(LoginUserId);
        ViewBag.userInstitutionId = user?.Employments.FirstOrDefault()?.InstitutionID;
        var careLogs = await _careGroomingLogManageService.GetLogsForInstitutionAsync(user.Employments.FirstOrDefault()?.InstitutionID);
        return View(careLogs);
    }

    public async Task<IActionResult> Details(int? logid)
    {
        if (logid == null) return NotFound();

        var caregroominglog = await _careGroomingLogManageService.GetDetailsAsync(logid.Value);
        if (caregroominglog == null) return NotFound();

        return View(caregroominglog);
    }

    public async Task<IActionResult> Create(int petid = 0)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var data = await _careGroomingLogManageService.GetCreateDataAsync(LoginUserId, petid);

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

        var result = await _careGroomingLogManageService.CreateAsync(caregroominglog, LoginUserId);
        if (!result.IsSuccess) return View(caregroominglog);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? logid, bool fromDetails = false)
    {
        if (logid == null) return NotFound();

        var caregroominglog = await _careGroomingLogManageService.GetEditDataAsync(logid.Value);
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

        var result = await _careGroomingLogManageService.UpdateAsync(logid.Value, caregroominglog, LoginUserId);

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

        var caregroominglog = await _careGroomingLogManageService.GetDeleteDataAsync(logid.Value);
        if (caregroominglog == null) return NotFound();

        return View(caregroominglog);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? logid)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();
        if (logid == null) return NotFound();

        var result = await _careGroomingLogManageService.DeleteAsync(logid.Value, LoginUserId);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();
        if (!result.IsSuccess && result.ErrorMessage == "FORBIDDEN") return Forbid();

        return RedirectToAction(nameof(Index));
    }
}