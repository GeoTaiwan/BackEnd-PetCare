using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.Controllers;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

// 機構端
[Authorize(Roles = "VetManage,BeautyManage,NannyManage,Vet,Beauty,Nanny")]
public class MedicalRecordManageController : BaseController
{
    private readonly IMedicalRecordManageService _medicalRecordManageService;

    public MedicalRecordManageController(IMedicalRecordManageService medicalRecordManageService)
    {
        _medicalRecordManageService = medicalRecordManageService;
    }

    public async Task<IActionResult> Index(int petid = 0)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var data = await _medicalRecordManageService.GetIndexDataAsync(LoginUserId, petid);
        if (data == null) return NotFound();

        ViewBag.InstitutionID = data.InstitutionID;
        ViewBag.InstitutionName = data.InstitutionName;
        return View(data.Records);
    }

    public async Task<IActionResult> Details(int? petid, string institutionID)
    {
        if (petid == null || string.IsNullOrEmpty(institutionID)) return NotFound();

        var records = await _medicalRecordManageService.GetDetailsByPetAsync(petid.Value, institutionID);
        if (records == null) return NotFound();

        return View(records);
    }

    public async Task<IActionResult> Create(int petid = 0, bool fromDetails = false)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        var data = await _medicalRecordManageService.GetCreateDataAsync(LoginUserId, petid);
        if (data == null) return BadRequest();

        ViewBag.FromDetails = fromDetails;
        ViewBag.PetID = data.PetID;
        ViewBag.PetName = data.PetName;
        ViewBag.InstitutionID = data.InstitutionID;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedicalRecord medicalrecord, IFormFile? uploadFile, bool fromDetails = false)
    {
        if (string.IsNullOrEmpty(LoginUserId)) return Unauthorized();

        ModelState.Remove(nameof(MedicalRecord.VetID));
        if (!ModelState.IsValid) return View(medicalrecord);

        var result = await _medicalRecordManageService.CreateAsync(medicalrecord, uploadFile, LoginUserId);
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
        var result = await _medicalRecordManageService.DeleteFileAsync(recordid);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        if (fromdetails) return RedirectToAction(nameof(Details), new { id = recordid });
        return RedirectToAction(nameof(Edit), new { recordid });
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile uploadFile, int recordid, bool fromdetails = false)
    {
        var result = await _medicalRecordManageService.UploadFileAsync(recordid, uploadFile);

        if (!result.IsSuccess && result.ErrorMessage == "NO_FILE") return BadRequest("No file uploaded.");
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        if (fromdetails) return RedirectToAction(nameof(Details), new { id = recordid });
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? recordid, bool fromDetails = false)
    {
        if (recordid == null) return NotFound();

        var medicalrecord = await _medicalRecordManageService.GetEditDataAsync(recordid.Value);
        if (medicalrecord == null) return NotFound();

        ViewBag.FromDetails = fromDetails;
        return View(medicalrecord);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? recordid, MedicalRecord medicalrecord, IFormFile? uploadFile, string? existingFileName, bool fromDetails = false)
    {
        if (recordid != medicalrecord.RecordID) return NotFound();

        ModelState.Remove("Pet");
        ModelState.Remove("Vet");
        ModelState.Remove("Vet.Name");

        if (!ModelState.IsValid) return View(medicalrecord);

        var result = await _medicalRecordManageService.UpdateAsync(recordid!.Value, medicalrecord, uploadFile, existingFileName);
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

        var medicalrecord = await _medicalRecordManageService.GetDeleteDataAsync(recordid.Value);
        if (medicalrecord == null) return NotFound();

        return View(medicalrecord);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null) return NotFound();

        var result = await _medicalRecordManageService.DeleteAsync(id.Value);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult DownloadFile(string fileName)
    {
        var result = _medicalRecordManageService.GetDownloadFilePath(fileName);

        if (!result.IsSuccess && result.ErrorMessage == "EMPTY_FILE_NAME")
            return NotFound("沒有提供檔名。");

        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND")
            return NotFound($"伺服器上找不到檔案: {fileName}");

        return PhysicalFile(result.Data, "application/octet-stream", fileName);
    }
}