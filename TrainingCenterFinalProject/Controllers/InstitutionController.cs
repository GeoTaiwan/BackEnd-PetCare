using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Controllers;
using System.Security.Claims;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class InstitutionController : BaseController
{
    private readonly IInstitutionService _institutionService;

    public InstitutionController(IInstitutionService institutionService)
    {
        _institutionService = institutionService;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var institutions = await _institutionService.GetMyInstitutionsAsync(LoginUserId);
        return View(institutions);
    }

    public async Task<IActionResult> IndexForReservation(string? typeId)
    {
        var institutions = await _institutionService.GetInstitutionsForReservationAsync(typeId);
        ViewBag.CurrentType = typeId;
        return View(institutions);
    }

    public async Task<IActionResult> InstitutionManage(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var institution = await _institutionService.GetInstitutionManageAsync(id);
        if (institution == null) return NotFound();

        return View(institution);
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var institution = await _institutionService.GetDetailsAsync(id);
        if (institution == null) return NotFound();

        return View(institution);
    }

    public async Task<IActionResult> Create()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var check = await _institutionService.CheckCreateEligibilityAsync(currentUserId);

        if (check.HasPendingApplication)
        {
            return RedirectToAction("Details", new { id = check.PendingInstitutionId });
        }

        await PopulateInstitutionTypesAndStatusesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Institution institution)
    {
        if (institution == null) return BadRequest();

        ModelState.Remove("InstitutionID");
        ModelState.Remove("StatusId");
        ModelState.Remove("OwnerID");

        if (!ModelState.IsValid)
        {
            await PopulateInstitutionTypesAndStatusesAsync();
            ModelState.AddModelError("", "請確認所有欄位皆已正確填寫。");
            return View(institution);
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _institutionService.CreateAsync(institution, currentUserId);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage == "ALREADY_OWNER")
                ModelState.AddModelError("", "您已經是某機構的負責人，無法重複申請。");
            else
                ModelState.AddModelError("", "系統忙碌，請稍後再試一次。");

            await PopulateInstitutionTypesAndStatusesAsync();
            return View(institution);
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Edit(string? id, bool fromDetails = false)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var institution = await _institutionService.GetByIdAsync(id);
        if (institution == null) return NotFound();

        await PopulateInstitutionTypesAndStatusesAsync();
        ViewBag.FromDetails = fromDetails;
        return View(institution);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Institution institution, bool fromDetails = false)
    {
        if (institution == null) return NotFound();

        if (fromDetails)
        {
            ModelState.Remove("InstitutionID");
            ModelState.Remove("StatusId");
            ModelState.Remove("OwnerID");
        }

        if (!ModelState.IsValid)
        {
            await PopulateInstitutionTypesAndStatusesAsync();
            return View(institution);
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _institutionService.UpdateAsync(institution, fromDetails, currentUserId);

        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND")
            return NotFound();

        if (fromDetails)
            return RedirectToAction("Details", new { id = result.Data.InstitutionID });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var result = await _institutionService.DeleteAsync(id);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateInstitutionTypesAndStatusesAsync()
    {
        ViewBag.InstitutionTypes = new SelectList(await _institutionService.GetInstitutionTypeSelectListAsync(), "Value", "Text");
        ViewBag.InstitutionStatuses = new SelectList(await _institutionService.GetInstitutionStatusSelectListAsync(), "Value", "Text");
    }
}
