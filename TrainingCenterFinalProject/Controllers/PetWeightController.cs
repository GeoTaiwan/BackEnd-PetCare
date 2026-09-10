using Microsoft.AspNetCore.Mvc;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class PetWeightController : BaseController
{
    private readonly IPetWeightService _petWeightService;

    public PetWeightController(IPetWeightService petWeightService)
    {
        _petWeightService = petWeightService;
    }

    // GET: PETWEIGHTS
    public async Task<IActionResult> Index(int petid = 0)
    {
        if (petid == 0) return BadRequest();
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var petWeights = await _petWeightService.GetByPetForOwnerAsync(petid, LoginUserId);
        return View(petWeights);
    }

    public IActionResult Create(int petid = 0)
    {
        if (petid == 0) return NotFound();

        ViewData["PetID"] = petid;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PetWeight petweight, bool fromIndex = false)
    {
        if (!ModelState.IsValid) return View(petweight);

        var result = await _petWeightService.CreateAsync(petweight);
        if (!result.IsSuccess) return View(petweight);

        if (fromIndex)
        {
            return RedirectToAction(nameof(Index), new { petid = petweight.PetID });
        }

        return RedirectToAction("Details", nameof(PetController).Replace("Controller", ""), new { petid = petweight.PetID });
    }

    // GET: PETWEIGHTS/Edit/5
    public async Task<IActionResult> Edit(int? petweightid)
    {
        if (petweightid == null) return NotFound();

        var petweight = await _petWeightService.GetByIdAsync(petweightid.Value);
        if (petweight == null) return NotFound();

        return View(petweight);
    }

    // POST: PETWEIGHTS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? petweightid, decimal weight)
    {
        if (petweightid == null) return NotFound();

        var result = await _petWeightService.UpdateWeightAsync(petweightid.Value, weight);

        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND")
            return NotFound();

        // 維持原流程：回到該寵物的體重列表
        return RedirectToAction(nameof(Index), new { petid = result.Data.PetID });
    }

    // POST: PETWEIGHTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? petweightid)
    {
        if (petweightid == null) return NotFound();

        var result = await _petWeightService.DeleteAsync(petweightid.Value);

        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND")
            return NotFound();

        return RedirectToAction(nameof(Index), new { petid = result.Data });
    }
}
