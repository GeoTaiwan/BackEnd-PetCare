using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

[Authorize(Roles = "VetManage,BeautyManage,NannyManage")]
public class InstitutionManageController : BaseController
{
    private readonly IAdminInstitutionService _adminInstitutionService;

    public InstitutionManageController(IAdminInstitutionService adminInstitutionService)
    {
        _adminInstitutionService = adminInstitutionService;
    }

    // GET: INSTITUTIONS
    public async Task<IActionResult> Index()
    {
        var institutions = await _adminInstitutionService.GetAllAsync();
        return View(institutions);
    }

    // GET: INSTITUTIONS/Create
    public async Task<IActionResult> Create()
    {
        await PopulateInstitutionTypesAndStatusesAsync();
        return View();
    }

    // POST: INSTITUTIONS/Create
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
            return View(institution);
        }

        var result = await _adminInstitutionService.CreateInstitutionAsync(institution, LoginUserId);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            await PopulateInstitutionTypesAndStatusesAsync();
            return View(institution);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: INSTITUTIONS/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var institution = await _adminInstitutionService.GetByIdAsync(id);
        if (institution == null) return NotFound();

        await PopulateInstitutionTypesAndStatusesAsync();
        return View(institution);
    }

    // POST: INSTITUTIONS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? id, Institution institution)
    {
        if (string.IsNullOrEmpty(id) || id != institution.InstitutionID) return NotFound();
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        if (!ModelState.IsValid)
        {
            await PopulateInstitutionTypesAndStatusesAsync();
            return View(institution);
        }

        var result = await _adminInstitutionService.UpdateInstitutionAsync(institution, false, LoginUserId);
        if (!result.IsSuccess)
        {
            if (result.ErrorMessage == "NOT_FOUND") return NotFound();

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            await PopulateInstitutionTypesAndStatusesAsync();
            return View(institution);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: INSTITUTIONS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var result = await _adminInstitutionService.DeleteInstitutionAsync(id);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

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