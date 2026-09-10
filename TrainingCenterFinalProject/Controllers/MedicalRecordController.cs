using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

// 飼主用 controller
public class MedicalRecordController : BaseController
{
    private readonly IMedicalRecordService _medicalRecordService;

    public MedicalRecordController(IMedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    public async Task<IActionResult> Index(int petid = 0)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var data = await _medicalRecordService.GetIndexDataAsync(LoginUserId, petid);

        ViewBag.noPet = data.NoPet;
        ViewBag.PetList = new SelectList(data.Pets, "PetID", "Name", data.SelectedPetId);
        ViewData["petid"] = data.SelectedPetId;

        return View(data.Records);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var medicalRecord = await _medicalRecordService.GetDetailsAsync(id.Value);
        if (medicalRecord == null) return NotFound();

        return View(medicalRecord);
    }

    public async Task<IActionResult> Create(int petid = 0, bool fromDetails = false)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var data = await _medicalRecordService.GetCreateDataAsync(LoginUserId, petid);
        if (data.NoPet) return NotFound();

        ViewBag.PetList = new SelectList(data.Pets, "PetID", "Name", data.SelectedPetId);
        ViewBag.FromDetails = fromDetails;
        ViewBag.PetID = data.SelectedPetId;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedicalRecord medicalrecord, IFormFile? uploadFile, bool fromDetails = false)
    {
        ModelState.Remove(nameof(MedicalRecord.VetID));

        if (!ModelState.IsValid)
        {
            if (!string.IsNullOrEmpty(LoginUserId))
            {
                var data = await _medicalRecordService.GetCreateDataAsync(LoginUserId, medicalrecord.PetID);
                ViewBag.PetList = new SelectList(data.Pets, "PetID", "Name", medicalrecord.PetID);
            }

            return View(medicalrecord);
        }

        var result = await _medicalRecordService.CreateAsync(medicalrecord, uploadFile);
        if (!result.IsSuccess) return View(medicalrecord);

        if (fromDetails)
        {
            return RedirectToAction(nameof(Details), nameof(PetController).Replace("Controller", ""), new { petid = medicalrecord.PetID });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFile(int recordid, bool fromdetails = false)
    {
        var result = await _medicalRecordService.DeleteFileAsync(recordid);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        if (fromdetails) return RedirectToAction(nameof(Details), new { id = recordid });
        return RedirectToAction(nameof(Edit), new { recordid });
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile uploadFile, int recordid, bool fromdetails = false)
    {
        var result = await _medicalRecordService.UploadFileAsync(recordid, uploadFile);

        if (!result.IsSuccess && result.ErrorMessage == "NO_FILE")
            return BadRequest("No file uploaded.");

        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND")
            return NotFound();

        if (fromdetails) return RedirectToAction(nameof(Details), new { id = recordid });
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? recordid, bool fromDetails = false)
    {
        if (recordid == null) return NotFound();

        var medicalRecord = await _medicalRecordService.GetEditDataAsync(recordid.Value);
        if (medicalRecord == null) return NotFound();

        ViewBag.FromDetails = fromDetails;
        return View(medicalRecord);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? recordid, MedicalRecord medicalrecord, IFormFile? uploadFile, string? existingFileName, bool fromDetails = false)
    {
        if (recordid == null) return NotFound();

        ModelState.Remove("Pet");
        ModelState.Remove("Vet");
        ModelState.Remove("Vet.Name");

        if (!ModelState.IsValid) return View(medicalrecord);

        var result = await _medicalRecordService.UpdateAsync(recordid.Value, medicalrecord, uploadFile, existingFileName);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        if (fromDetails)
        {
            return RedirectToAction(nameof(Details), nameof(PetController).Replace("Controller", ""), new { petid = medicalrecord.PetID });
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? recordid)
    {
        if (recordid == null) return NotFound();

        var medicalRecord = await _medicalRecordService.GetDeleteDataAsync(recordid.Value);
        if (medicalRecord == null) return NotFound();

        return View(medicalRecord);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null) return NotFound();

        var result = await _medicalRecordService.DeleteAsync(id.Value);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult DownloadFile(string fileName)
    {
        var result = _medicalRecordService.GetDownloadFilePath(fileName);

        if (!result.IsSuccess && result.ErrorMessage == "EMPTY_FILE_NAME")
            return NotFound("沒有提供檔名。");

        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND")
            return NotFound($"伺服器上找不到檔案: {fileName}");

        return PhysicalFile(result.Data, "application/octet-stream", fileName);
    }
}