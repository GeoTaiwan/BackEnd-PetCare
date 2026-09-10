using Microsoft.AspNetCore.Mvc;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

//[Authorize]
public class UserProfileController : Controller
{
    private readonly IUserProfileService _userProfileService;

    public UserProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _userProfileService.GetAllAsync());
    }

    public async Task<IActionResult> Details(string? userid)
    {
        if (string.IsNullOrEmpty(userid)) return NotFound();

        var userprofile = await _userProfileService.GetByIdAsync(userid);
        if (userprofile == null) return NotFound();

        return View(userprofile);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserID,Name,Phone,Authorization,Gender,DateOfBirth,Address,JoinDate,Status,Email,CredentialDetails,Employments,Pets,VetMedicalRecords,StaffCareLogs,StaffTaskSchedules")] UserProfile userprofile)
    {
        if (!ModelState.IsValid) return View(userprofile);

        var result = await _userProfileService.CreateAsync(userprofile);
        if (!result.IsSuccess) return View(userprofile);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? userid)
    {
        if (string.IsNullOrEmpty(userid)) return NotFound();

        var userprofile = await _userProfileService.GetByIdAsync(userid);
        if (userprofile == null) return NotFound();

        return View(userprofile);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string? userid, [Bind("UserID,Name,Phone,Authorization,Gender,DateOfBirth,Address,JoinDate,Status,Email,CredentialDetails,Employments,Pets,VetMedicalRecords,StaffCareLogs,StaffTaskSchedules")] UserProfile userprofile)
    {
        if (string.IsNullOrEmpty(userid)) return NotFound();
        if (!ModelState.IsValid) return View(userprofile);

        var result = await _userProfileService.UpdateAsync(userid, userprofile);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string? userid)
    {
        if (string.IsNullOrEmpty(userid)) return NotFound();

        var userprofile = await _userProfileService.GetByIdAsync(userid);
        if (userprofile == null) return NotFound();

        return View(userprofile);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? userid)
    {
        if (string.IsNullOrEmpty(userid)) return NotFound();

        var result = await _userProfileService.DeleteAsync(userid);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }
}
