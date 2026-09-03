using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingCenterFinalProject.Services;
using TrainingCenterFinalProject.ViewModels;

namespace TrainingCenterFinalProject.Controllers;

[Authorize(Roles = "Admin")]
public class AdminUserController : Controller
{
    private readonly IAdminUserService _adminUserService;

    public AdminUserController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateRolesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminUserCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateRolesAsync();
            return View(model);
        }

        var result = await _adminUserService.CreateUserAsync(model);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError("Email", result.ErrorMessage);
            await PopulateRolesAsync();
            return View(model);
        }

        TempData["SuccessMessage"] = $"成功建立使用者，帳號代碼為：{result.Data}";
        return RedirectToAction("Index", "UserProfile");
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string userid)
    {
        if (string.IsNullOrEmpty(userid)) return NotFound();

        var model = await _adminUserService.GetResetPasswordModelAsync(userid);
        if (model == null) return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(AdminResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _adminUserService.ResetPasswordAsync(model);
        if (!result.IsSuccess)
        {
            if (result.ErrorMessage == "NOT_FOUND") return NotFound();
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        TempData["SuccessMessage"] = $"已經成功將 {model.UserName} 的密碼重設！";
        return RedirectToAction(nameof(Index), "UserProfile");
    }

    private async Task PopulateRolesAsync()
    {
        var roles = await _adminUserService.GetRolesAsync();
        ViewBag.Authlist = new SelectList(roles, "Id", "RoleName");
    }
}