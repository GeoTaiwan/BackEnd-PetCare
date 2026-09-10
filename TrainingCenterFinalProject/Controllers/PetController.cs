using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

//[Authorize(Roles = "Member,VetManage")]
public class PetController : BaseController
{
    private readonly IPetService _petService;
    private readonly IWebHostEnvironment _env;

    public PetController(IPetService petService, IWebHostEnvironment env)
    {
        _petService = petService;
        _env = env;
    }

    public async Task<IActionResult> Index(string userid)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var myPets = await _petService.GetUserPetsAsync(LoginUserId);
        return View(myPets);
    }

    public async Task<IActionResult> Details(int? petid)
    {
        if (petid == null) return NotFound();

        var pet = await _petService.GetDetailsAsync(petid.Value);
        if (pet == null) return NotFound();

        return View(pet);
    }

    public IActionResult Create(bool fromMR = false)
    {
        ViewBag.FromMR = fromMR;
        ViewBag.GenderList = new SelectList(_petService.PopulateGenderList());
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PetID,Name,Species,Breed,Gender,DateOfBirth,MicrochipNo,SpecialNotes,UploadPhoto")] Pet pet, bool fromMR = false)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        ModelState.Remove("OwnerID");
        if (!ModelState.IsValid) return View(pet);

        var result = await _petService.CreateAsync(pet, LoginUserId, _env.WebRootPath);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "建立寵物失敗。");
            return View(pet);
        }

        if (fromMR)
        {
            return RedirectToAction(nameof(Index), nameof(MedicalRecordManageController).Replace("Controller", ""));
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? petid, bool fromdetails = false)
    {
        if (petid == null || string.IsNullOrEmpty(LoginUserId)) return NotFound();

        var pet = await _petService.GetOwnedPetAsync(petid.Value, LoginUserId);
        if (pet == null) return NotFound();

        ViewBag.FromDetails = fromdetails;
        ViewBag.PetID = petid;
        return View(pet);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? petid, [Bind("PetID,Name,Species,Breed,Gender,DateOfBirth,MicrochipNo,SpecialNotes,ProfilePhotoUrl,UploadPhoto")] Pet pet, bool fromdetails = false)
    {
        if (petid == null || string.IsNullOrEmpty(LoginUserId)) return NotFound();

        ModelState.Remove("OwnerID");
        if (!ModelState.IsValid) return View(pet);

        var result = await _petService.UpdateAsync(petid.Value, pet, LoginUserId, _env.WebRootPath);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        if (fromdetails)
        {
            return RedirectToAction(nameof(Details), nameof(PetController).Replace("Controller", ""), new { petid = pet.PetID });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? petid)
    {
        if (petid == null) return NotFound();

        var result = await _petService.DeleteAsync(petid.Value, _env.WebRootPath);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int petid)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var result = await _petService.DeletePhotoAsync(petid, LoginUserId, _env.WebRootPath);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Edit), new { petid });
    }
}
