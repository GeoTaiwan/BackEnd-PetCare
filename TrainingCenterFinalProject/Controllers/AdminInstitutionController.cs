using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

namespace TrainingCenterFinalProject.Controllers;

[Authorize(Roles = "Admin")]
public class AdminInstitutionController : BaseController
{
    private readonly IAdminInstitutionService _adminInstitutionService;
    private readonly IInstitutionService _institutionService;

    public AdminInstitutionController(IAdminInstitutionService adminInstitutionService, IInstitutionService institutionService)
    {
        _adminInstitutionService = adminInstitutionService;
        _institutionService = institutionService;
    }

    public async Task<IActionResult> Index()
    {
        var institutions = await _adminInstitutionService.GetAllAsync();
        return View(institutions);
    }

    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var institution = await _adminInstitutionService.GetDetailsAsync(id);
        if (institution == null) return NotFound();

        return View(institution);
    }

    public async Task<IActionResult> Create()
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var pending = await _adminInstitutionService.GetPendingInstitutionByOwnerAsync(LoginUserId);
        if (pending != null)
        {
            return RedirectToAction("Details", new { id = pending.InstitutionID });
        }

        await PopulateInstitutionTypesAndStatusesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Institution institution)
    {
        if (institution == null) return BadRequest();
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        ModelState.Remove("InstitutionID");
        ModelState.Remove("StatusId");
        ModelState.Remove("OwnerID");

        if (!ModelState.IsValid)
        {
            await PopulateInstitutionTypesAndStatusesAsync();
            ModelState.AddModelError("", "請確認所有欄位皆已正確填寫。");
            return View(institution);
        }

        var result = await _adminInstitutionService.CreateInstitutionAsync(institution, LoginUserId);
        if (result.IsSuccess) return RedirectToAction("Index", "Home");

        ModelState.AddModelError("", result.ErrorMessage);
        await PopulateInstitutionTypesAndStatusesAsync();
        return View(institution);
    }

    public async Task<IActionResult> Edit(string? id, bool fromDetails = false)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var institution = await _adminInstitutionService.GetByIdAsync(id);
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
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

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

        var result = await _adminInstitutionService.UpdateInstitutionAsync(institution, fromDetails, LoginUserId);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage == "NOT_FOUND") return NotFound();

            ModelState.AddModelError("", result.ErrorMessage);
            await PopulateInstitutionTypesAndStatusesAsync();
            return View(institution);
        }

        if (fromDetails)
        {
            return RedirectToAction("Details", new { id = result.Data.InstitutionID });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string institutionId, bool fromDetails = false)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var institution = await _institutionService.GetInstitutionManageAsync(institutionId);
        institution.StatusId = "S01";
        var result = await _adminInstitutionService.UpdateInstitutionAsync(institution, fromDetails, LoginUserId);

        //if (!result.IsSuccess)
        //{
        //    if (result.ErrorMessage == "NOT_FOUND") return NotFound();

        //    ModelState.AddModelError("", result.ErrorMessage);
        //    await PopulateInstitutionTypesAndStatusesAsync();
        //    return View(institution);
        //}

        if (fromDetails)
        {
            return RedirectToAction("Details", new { id = result.Data.InstitutionID });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var result = await _adminInstitutionService.DeleteInstitutionAsync(id);
        if (!result.IsSuccess) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateInstitutionTypesAndStatusesAsync()
    {
        var types = await _adminInstitutionService.GetInstitutionTypesAsync();
        var statuses = await _adminInstitutionService.GetInstitutionStatusesAsync();

        ViewBag.InstitutionTypes = new SelectList(types, "TypeId", "Name");
        ViewBag.InstitutionStatuses = new SelectList(statuses, "StatusId", "StatusName");
    }
}