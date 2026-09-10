using Microsoft.AspNetCore.Mvc;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class AuthorizationRecordController : BaseController
{
    private readonly IAuthorizationRecordService _authorizationRecordService;

    public AuthorizationRecordController(IAuthorizationRecordService authorizationRecordService)
    {
        _authorizationRecordService = authorizationRecordService;
    }

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var ars = await _authorizationRecordService.GetUserRecordsAsync(LoginUserId);
        return View(ars);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(string? dataauthorizationnumber, bool isAuthorized = false)
    {
        if (string.IsNullOrEmpty(dataauthorizationnumber)) return NotFound();

        var ok = await _authorizationRecordService.ChangeStatusAsync(dataauthorizationnumber, isAuthorized);
        if (!ok) return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string? dataauthorizationnumber)
    {
        if (string.IsNullOrEmpty(dataauthorizationnumber)) return NotFound();

        var record = await _authorizationRecordService.GetByIdAsync(dataauthorizationnumber);
        if (record == null) return NotFound();

        return View(record);
    }

    public async Task<IActionResult> Create(int petid, string institutionid)
    {
        var createData = await _authorizationRecordService.GetCreateDataAsync(petid, institutionid);
        if (createData == null) return BadRequest();

        ViewBag.PetName = createData.PetName;
        ViewBag.InstName = createData.InstitutionName;
        return View(createData.Record);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorizationRecord authorizationrecord)
    {
        ModelState.Remove("DataAuthorizationNumber");
        ModelState.Remove("Pet");
        ModelState.Remove("Institution");
        ModelState.Remove("Status");

        if (!ModelState.IsValid) return View(authorizationrecord);

        var result = await _authorizationRecordService.CreateAsync(authorizationrecord);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "建立授權失敗。");
            return View(authorizationrecord);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? dataauthorizationnumber)
    {
        if (string.IsNullOrEmpty(dataauthorizationnumber)) return NotFound();

        var record = await _authorizationRecordService.GetByIdAsync(dataauthorizationnumber);
        if (record == null) return NotFound();

        return View(record);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? dataauthorizationnumber, [Bind("DataAuthorizationNumber,PetID,InstitutionID,GrantedDate,RevokedDate,Status,Pet,Institution")] AuthorizationRecord authorizationrecord)
    {
        if (string.IsNullOrEmpty(dataauthorizationnumber)) return NotFound();
        if (!ModelState.IsValid) return View(authorizationrecord);

        var result = await _authorizationRecordService.UpdateAsync(dataauthorizationnumber, authorizationrecord);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string? dataauthorizationnumber)
    {
        if (string.IsNullOrEmpty(dataauthorizationnumber)) return NotFound();

        var record = await _authorizationRecordService.GetByIdAsync(dataauthorizationnumber);
        if (record == null) return NotFound();

        return View(record);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? dataauthorizationnumber)
    {
        if (string.IsNullOrEmpty(dataauthorizationnumber)) return NotFound();

        var ok = await _authorizationRecordService.DeleteAsync(dataauthorizationnumber);
        if (!ok) return NotFound();

        return RedirectToAction(nameof(Index));
    }
}
