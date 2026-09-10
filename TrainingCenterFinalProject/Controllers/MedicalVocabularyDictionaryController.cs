using Microsoft.AspNetCore.Mvc;
using TrainingCenterFinalProject.Models;
using TrainingCenterFinalProject.Services;

public class MedicalVocabularyDictionaryController : Controller
{
    private readonly IMedicalVocabularyDictionaryService _service;

    public MedicalVocabularyDictionaryController(IMedicalVocabularyDictionaryService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _service.GetAllAsync();
        return View(data);
    }

    public async Task<IActionResult> Details(int? termid)
    {
        if (termid == null) return NotFound();

        var entity = await _service.GetByIdAsync(termid.Value);
        if (entity == null) return NotFound();

        return View(entity);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedicalVocabularyDictionary medicalvocabularydictionary)
    {
        if (medicalvocabularydictionary == null) return NotFound();

        ModelState.Remove(nameof(MedicalVocabularyDictionary.UpdatedAt));
        if (!ModelState.IsValid) return View(medicalvocabularydictionary);

        var result = await _service.CreateAsync(medicalvocabularydictionary);
        if (!result.IsSuccess) return View(medicalvocabularydictionary);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var entity = await _service.GetByIdAsync(id.Value);
        if (entity == null) return NotFound();

        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, MedicalVocabularyDictionary medicalvocabularydictionary)
    {
        if (id == null) return NotFound();

        ModelState.Remove(nameof(MedicalVocabularyDictionary.UpdatedAt));
        if (!ModelState.IsValid) return View(medicalvocabularydictionary);

        var result = await _service.UpdateAsync(id.Value, medicalvocabularydictionary);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var entity = await _service.GetByIdAsync(id.Value);
        if (entity == null) return NotFound();

        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null) return NotFound();

        var result = await _service.DeleteAsync(id.Value);
        if (!result.IsSuccess && result.ErrorMessage == "NOT_FOUND") return NotFound();

        return RedirectToAction(nameof(Index));
    }
}