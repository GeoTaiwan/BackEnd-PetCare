using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Controllers;
using PetCare.ViewModels;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

[Authorize(Roles = "Vet,Beauty,Nanny,VetManage,BeautyManage,NannyManage")]
public class EmploymentController : BaseController
{
    private readonly IEmploymentService _employmentService;

    public EmploymentController(IEmploymentService employmentService)
    {
        _employmentService = employmentService;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var instID = await _employmentService.GetInstitutionIdByUserIdAsync(LoginUserId);
        if (string.IsNullOrEmpty(instID)) return BadRequest();

        var employments = await _employmentService.GetEmploymentsByInstitutionAsync(instID);
        ViewBag.InstitutionName = await _employmentService.GetInstitutionNameAsync(instID);

        return View(employments);
    }

    public async Task<IActionResult> Create(string instid, bool fromInstManage = false)
    {
        var vm = await _employmentService.BuildCreateViewModelAsync(instid);
        if (vm == null) return NotFound();

        await PopulateDropdownsAsync();
        ViewBag.fromInstManage = fromInstManage;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewEmployeeViewModel emViewModel, bool fromInstManage = false)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            ViewBag.fromInstManage = fromInstManage;
            return View(emViewModel);
        }

        var result = await _employmentService.CreateAsync(emViewModel);

        if (!result.IsSuccess)
        {
            if (result.ErrorMessage == "USER_NOT_FOUND")
                ModelState.AddModelError(nameof(NewEmployeeViewModel.UserEmail), "找不到此信箱對應的會員。");
            else if (result.ErrorMessage == "EMPLOYMENT_EXISTS")
                ModelState.AddModelError(string.Empty, "此員工已在該機構任職。");
            else
                ModelState.AddModelError(string.Empty, "新增員工失敗。");

            await PopulateDropdownsAsync();
            ViewBag.fromInstManage = fromInstManage;
            return View(emViewModel);
        }

        if (fromInstManage)
        {
            return RedirectToAction("InstitutionManage", "Institution", new { id = emViewModel.InstitutionID });
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(LoginUserId)) return NotFound();

        var employment = await _employmentService.GetEmploymentForEditAsync(LoginUserId, id);
        if (employment == null) return NotFound();

        await PopulateDropdownsAsync();
        return View(employment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? id, Employment employment)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(LoginUserId)) return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            return View(employment);
        }

        var result = await _employmentService.UpdateAsync(LoginUserId, id, employment);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string? userid)
    {
        if (string.IsNullOrEmpty(userid)) return NotFound();

        var employment = await _employmentService.GetByUserIdAsync(userid);
        if (employment == null) return NotFound();

        return View(employment);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? userid)
    {
        if (string.IsNullOrEmpty(userid)) return NotFound();

        var result = await _employmentService.DeleteAsync(userid);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        ViewBag.OccupationList = new SelectList(await _employmentService.GetOccupationSelectListAsync(), "Value", "Text");
        ViewBag.StatusList = new SelectList(_employmentService.GetStatusSelectList(), "Value", "Text");
    }
}