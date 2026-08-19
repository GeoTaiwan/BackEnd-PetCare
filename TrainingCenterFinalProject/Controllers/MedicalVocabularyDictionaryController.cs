
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenterFinalProject.Data;
using TrainingCenterFinalProject.Models;

public class MedicalVocabularyDictionaryController : Controller
{
    private readonly PetCareDbContext _context;

    public MedicalVocabularyDictionaryController(PetCareDbContext context)
    {
        _context = context;
    }

    // GET: MEDICALVOCABULARYDICTIONARYS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.MedicalVocabularyDictionaries.ToListAsync());
    }

    // GET: MEDICALVOCABULARYDICTIONARYS/Details/5
    public async Task<IActionResult> Details(int? termid)
    {
        if (termid == null)
        {
            return NotFound();
        }

        var medicalvocabularydictionary = await _context.MedicalVocabularyDictionaries
            .FirstOrDefaultAsync(m => m.TermID == termid);
        if (medicalvocabularydictionary == null)
        {
            return NotFound();
        }

        return View(medicalvocabularydictionary);
    }

    // GET: MEDICALVOCABULARYDICTIONARYS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MEDICALVOCABULARYDICTIONARYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedicalVocabularyDictionary medicalvocabularydictionary)
    {
        if (medicalvocabularydictionary == null) { 
            return NotFound();
        }
        ModelState.Remove("UpdatedAt");
        medicalvocabularydictionary.UpdatedAt = DateTime.Now;
        if (ModelState.IsValid)
        {
            _context.Add(medicalvocabularydictionary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(medicalvocabularydictionary);
    }

    // GET: MEDICALVOCABULARYDICTIONARYS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var medicalvocabularydictionary = await _context.MedicalVocabularyDictionaries.FindAsync(id);
        if (medicalvocabularydictionary == null)
        {
            return NotFound();
        }
        return View(medicalvocabularydictionary);
    }

    // POST: MEDICALVOCABULARYDICTIONARYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id,MedicalVocabularyDictionary medicalvocabularydictionary)
    {
        if (id != medicalvocabularydictionary.TermID)
        {
            return NotFound();
        }
        ModelState.Remove("UpdatedAt"); 
        medicalvocabularydictionary.UpdatedAt = DateTime.Now; 
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(medicalvocabularydictionary);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalVocabularyDictionaryExists(medicalvocabularydictionary.TermID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(medicalvocabularydictionary);
    }

    // GET: MEDICALVOCABULARYDICTIONARYS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var medicalvocabularydictionary = await _context.MedicalVocabularyDictionaries
            .FirstOrDefaultAsync(m => m.TermID == id);
        if (medicalvocabularydictionary == null)
        {
            return NotFound();
        }

        return View(medicalvocabularydictionary);
    }

    // POST: MEDICALVOCABULARYDICTIONARYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var medicalvocabularydictionary = await _context.MedicalVocabularyDictionaries.FindAsync(id);
        if (medicalvocabularydictionary != null)
        {
            _context.MedicalVocabularyDictionaries.Remove(medicalvocabularydictionary);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MedicalVocabularyDictionaryExists(int? termid)
    {
        return _context.MedicalVocabularyDictionaries.Any(e => e.TermID == termid);
    }
}
